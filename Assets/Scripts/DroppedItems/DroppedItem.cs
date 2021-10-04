using System.Collections.Generic;
using System.Linq;
using Net;
using UnityEngine;

public class DroppedItem : NetIdentity
{
  public bool immediate;

  public float radius;

  public LayerMask targets;

  static object lockPlayersList = new object();

  List<Player> _availablePlayers = new List<Player>();

  [SerializeField]
  protected SpriteRenderer display;

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
      _rendererTransform.localScale = _originalScale + Vector3.one * MathUtility.SineWave(.05f, 20, Time.time);
    }
  }

  void DetectAvailablePlayers()
  {
    var targetsFound = Physics.OverlapSphere(transform.position, radius, targets);
    if (targetsFound.Any())
    {
      var targetsOrdered = targetsFound
        .Where(x => x.GetComponent<Player>())
        .OrderBy(x => (transform.position - x.transform.position).sqrMagnitude)
        .Select(x => x.GetComponent<Player>());
      lock (lockPlayersList)
      {
        _availablePlayers = _availablePlayers.Where(x => x).ToList();
        foreach (var player in targetsOrdered.ToArray())
        {
          if (player)
          {
            if (_availablePlayers.All(x => x.id != player.id))
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
        var removedTargets = _availablePlayers.Where(x => targetsOrdered.Any(player => player.id != x.id));
        if (removedTargets.Any())
        {
          foreach (var removedTarget in removedTargets)
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

  public virtual void PickUp(Player player)
  {
    if (isServer)
    {
      EmitMessage("destroy_dropped_item", new DestroyDroppedItemJson());
      NetDestroy(this);
    }
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
