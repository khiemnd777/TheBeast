using System.Linq;
using UnityEngine;

public class CuriousGenerator : MonoBehaviour
{
  public event System.Action onAfterGenerate;
  public float radius;
  public LayerMask targetMask;
  public Transform affectedTransform;

  [Space]
  public Curiosity footstepPrefab;
  public float footstepRadius;
  public float footstepLifetime = 4f;

  void Start()
  {
    if (!affectedTransform)
    {
      affectedTransform = transform;
    }
  }

  public void Generate()
  {
    var targetsInRadius = Physics.OverlapSphere(affectedTransform.position, radius, targetMask);
    if (targetsInRadius.Length < 0 || targetsInRadius.All(target => !target))
    {
      var footstep = Instantiate<Curiosity>(footstepPrefab, transform.position, Quaternion.identity);
      footstep.radius = footstepRadius;
      footstep.lifetime = footstepLifetime;

      if (onAfterGenerate != null)
      {
        onAfterGenerate();
      }
    }
  }
}
