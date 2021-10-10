using System.Linq;
using Net;
using UnityEngine;

public class PickUpGunController : MonoBehaviour
{
  public Player player;

  [SerializeField]
  NetGunHolder _leftGunHolder;

  [SerializeField]
  NetGunHolder _rightGunHolder;

  GunRegistrar _gunRegistrar;

  void Start()
  {
    _gunRegistrar = FindObjectOfType<GunRegistrar>();
    if (player.isClient)
    {
      player.onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "pick_up_gun")
        {
          var dataJson = Utility.Deserialize<PickUpGunJson>(eventMessage);
          if (_gunRegistrar.fields != null)
          {
            var found = _gunRegistrar.fields.FirstOrDefault(x => x.name == dataJson.registeredName);
            if (found.prefab)
            {
              if (_rightGunHolder)
              {
                _rightGunHolder.KeepInCover();
                _rightGunHolder.gun = found.prefab;
                _rightGunHolder.TakeUpArm();
              }
              if (found.prefab.gunHandType == GunHandType.OneHand)
              {
                if (_leftGunHolder)
                {
                  _leftGunHolder.KeepInCover();
                  _leftGunHolder.gun = found.prefab;
                  _leftGunHolder.TakeUpArm();
                }
              }
            }
          }
        }
      };
    }
    if (player.isServer)
    {
      player.onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "pick_up_command")
        {
          // Player picks item
          var picker = player.GetComponent<IPicker>();
          if (picker != null)
          {
            var droppedItems = picker.droppedItems.Where(x => x.GetComponent<DroppedGun>()).Select(x => x.GetComponent<DroppedGun>());
            if (droppedItems.Any())
            {
              var droppedItemMatched = droppedItems.FirstOrDefault();
              if (droppedItemMatched)
              {
                // Picked by picker
                picker.PickUp(droppedItemMatched);
                // Picked by controller
                PickUp(droppedItemMatched, player.transform.position, Quaternion.identity, 1.25f);
              }
            }
          }
        }
      };
    }
  }

  void Update()
  {
    if (player && player.isLocal)
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        player.EmitMessage("pick_up_command", null, true);
      }
    }
  }

  public NetGun PickUp(DroppedGun droppedGun, Vector3 position, Quaternion rotation, float droppedRadius = 0)
  {
    if (player.isServer)
    {
      if (droppedGun)
      {
        // Drop the gun on hands
        var gunBeDropped = _rightGunHolder.gun?.droppedGun;
        if (gunBeDropped)
        {
          var spawnPosition = position + Random.insideUnitSphere * droppedRadius;
          NetIdentity.InstantiateServerAndEverywhere(gunBeDropped.prefabName, gunBeDropped, new Vector3(spawnPosition.x, 0, spawnPosition.z), rotation, null, null, true);
        }
        if (_rightGunHolder)
        {
          _rightGunHolder.KeepInCover();
          _rightGunHolder.gun = droppedGun.gunPrefab;
          _rightGunHolder.TakeUpArm();
        }
        if (droppedGun.gunPrefab.gunHandType == GunHandType.OneHand)
        {
          if (_leftGunHolder)
          {
            _leftGunHolder.KeepInCover();
            _leftGunHolder.gun = droppedGun.gunPrefab;
            _leftGunHolder.TakeUpArm();
          }
        }
        player.EmitMessage("pick_up_gun", new PickUpGunJson
        {
          registeredName = droppedGun.gunPrefab.registeredName
        });
      }
    }
    return null;
  }
}

public struct PickUpGunJson
{
  public string registeredName;
}