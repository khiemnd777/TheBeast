using System.Linq;
using UnityEngine;

public class FootstepGenerator : MonoBehaviour
{
  public float radius;
  public LayerMask targetMask;
  public Transform affectedTransform;

  [Space]
  public Footstep footstepPrefab;
  public float footstepRadius;
  public float footstepLifetime = 4f;

  public void Generate()
  {
    var targetsInRadius = Physics.OverlapSphere(affectedTransform.position, radius, targetMask);
    if (targetsInRadius.Length < 0 || targetsInRadius.All(target => !target))
    {
      var footstep = Instantiate<Footstep>(footstepPrefab, transform.position, Quaternion.identity);
      footstep.radius = footstepRadius;
      footstep.lifetime = footstepLifetime;
    }
  }
}
