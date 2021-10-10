using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Net;
using UnityEngine;

public class DroppedItem : NetIdentity
{
  [Header("General")]
  public bool immediate;

  public float radius;

  public float amplitude;

  public LayerMask targets;

  [SerializeField]
  protected SpriteRenderer display;

  [Header("Auto-sync")]
  public bool autoSync;

  public float syncDelayInSeconds;

  object lockPlayersList = new object();

  List<Player> _availablePlayers = new List<Player>();

  Transform _rendererTransform;

  Vector3 _originalScale;

  protected override void Start()
  {
    base.Start();
    if (isClient)
    {
      _rendererTransform = display.transform;
      _originalScale = _rendererTransform.localScale;
      onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "destroy_dropped_item")
        {
          NetDestroy(this);
        }
      };
    }
    if (isServer)
    {
      if (autoSync)
      {
        StartCoroutine("AutoSync", syncDelayInSeconds);
      }
    }
  }

  IEnumerator AutoSync(float syncDelayInSeconds)
  {
    while (gameObject)
    {
      yield return new WaitForSeconds(syncDelayInSeconds);
      EmitMessage("auto_sync_dropped_item", null);
    }
  }

  protected override void Update()
  {
    base.Update();
    if (isServer)
    {
      DetectAvailablePlayers();
      PickUpImmediately();
    }
    if (isClient)
    {
      _rendererTransform.localScale = _originalScale + Vector3.one * MathUtility.SineWave(amplitude, 20, Time.time);
    }
  }

  void DetectAvailablePlayers()
  {
    var targetsFound = Physics.OverlapSphere(transform.position, radius, targets);
    if (targetsFound.Any())
    {
      var targetsOrdered = targetsFound
        .Where(x => x.GetComponent<Player>())
        .Select(x => x.GetComponent<Player>())
        .Where(x => !x.lifeEnd)
        .OrderBy(x => (transform.position - x.transform.position).sqrMagnitude);
      if (targetsOrdered.Any())
      {
        lock (lockPlayersList)
        {
          _availablePlayers = _availablePlayers.Where(x => x).ToList();
          foreach (var player in targetsOrdered.ToArray())
          {
            if (player)
            {
              if (!_availablePlayers.Any(x => x.id == player.id))
              {
                var picker = player.GetComponent<IPicker>();
                if (picker != null)
                {
                  picker.AddDroppedItem(this);
                  _availablePlayers.Add(player);
                }
              }
            }
          }
          var removedTargets = _availablePlayers.Where(x => !targetsOrdered.Any(player => player.id == x.id));
          if (removedTargets.Any())
          {
            foreach (var removedTarget in removedTargets.ToList())
            {
              var picker = removedTarget?.GetComponent<IPicker>();
              if (picker != null)
              {
                picker.RemoveDroppedItem(this);
                _availablePlayers.Remove(removedTarget);
              }
            }
          }
        }
        return;
      }
    }
    // Clear all if not any found targets
    if (_availablePlayers.Any())
    {
      lock (lockPlayersList)
      {
        var removedTargets = _availablePlayers.ToList();
        foreach (var removedTarget in removedTargets)
        {
          if (removedTarget)
          {
            var picker = removedTarget.GetComponent<IPicker>();
            if (picker != null)
            {
              picker.RemoveDroppedItem(this);
              _availablePlayers.Remove(removedTarget);
            }
          }
        }
      }
    }
  }

  void PickUpImmediately()
  {
    if (immediate && _availablePlayers.Any())
    {
      var firstPlayer = _availablePlayers.FirstOrDefault();
      if (firstPlayer)
      {
        var picker = firstPlayer.GetComponent<IPicker>();
        if (picker != null)
        {
          picker.PickUp(this);
        }
      }
    }
  }

  object _pickUpLock = new object();

  public void PickUp(Player player)
  {
    if (isServer)
    {
      lock (_pickUpLock)
      {
        if (!gameObject) return;
        if (OnPickUp(player))
        {
          player.RemoveDroppedItem(this);
          EmitMessage("destroy_dropped_item", new DestroyDroppedItemJson());
          NetDestroy(this);
        }
      }
    }
  }

  public virtual bool OnPickUp(Player player)
  {
    return false;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, radius);
  }
}

public struct DestroyDroppedItemJson
{

}
