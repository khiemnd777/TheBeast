using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
  public float size;
  public float range;
  public LayerMask layerMask;
  public Player player;
  bool _isSetup;

  public void Collide(float damagePoint, float freezedTime, float hitbackPoint)
  {
    if (_isSetup)
    {
      var colliders = Physics.OverlapSphere(this.transform.position, size, layerMask);
      if (colliders.Any())
      {
        foreach (var collider in colliders)
        {
          var otherPlayer = collider.GetComponent<Player>();
          if (otherPlayer)
          {
            var impactedPositionNormalized = collider.ClosestPointOnBounds(transform.position);
            var impactedPoint = impactedPositionNormalized;
            impactedPositionNormalized.Normalize();
            otherPlayer.OnHittingUp(damagePoint, freezedTime, hitbackPoint, impactedPoint, impactedPositionNormalized, player.id, true);
          }
        }
      }
    }
  }

  public void Setup(MeleeColliderOptions options)
  {
    this.player = options.player;
    this.size = options.size;
    this.range = options.range;
    this.layerMask = options.layerMask;
    this.transform.localPosition = new Vector3(range, 0f, 0f);
    _isSetup = true;
  }

  public void Reset()
  {
    _isSetup = false;
    this.player = null;
    this.size = 0f;
    this.range = 0f;
    this.layerMask = -1;
    this.transform.localPosition = Vector3.zero;
  }

  void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(this.transform.position, size);
  }
}

public struct MeleeColliderOptions
{
  public float size;
  public float range;
  public LayerMask layerMask;
  public Player player;
}
