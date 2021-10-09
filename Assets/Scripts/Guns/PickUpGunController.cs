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
                _rightGunHolder.gun = found.prefab;
              }
              if (found.prefab.gunHandType == GunHandType.OneHand)
              {
                if (_leftGunHolder)
                {
                  _leftGunHolder.gun = found.prefab;
                }
              }
            }
          }
        }
      };
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
          _rightGunHolder.gun = droppedGun.gunPrefab;
        }
        if (droppedGun.gunPrefab.gunHandType == GunHandType.OneHand)
        {
          if (_leftGunHolder)
          {
            _leftGunHolder.gun = droppedGun.gunPrefab;
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