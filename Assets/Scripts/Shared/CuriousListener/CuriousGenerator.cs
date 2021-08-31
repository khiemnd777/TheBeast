using System.Linq;
using UnityEngine;

public class CuriousGenerator : MonoBehaviour
{
  public event System.Action onAfterGenerate;

  [System.NonSerialized]
  public string curiousIdentity;

  public float radius;
  public LayerMask targetMask;
  public Transform affectedTransform;

  [Space]
  public Curiosity curiousPrefab;
  public float curiousRadius;
  public float curiousLifetime = 4f;

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
      var curiosity = Instantiate<Curiosity>(curiousPrefab, transform.position, Quaternion.identity);
      curiosity.curiousIdentity = curiousIdentity;
      curiosity.radius = curiousRadius;
      curiosity.lifetime = curiousLifetime;

      if (onAfterGenerate != null)
      {
        onAfterGenerate();
      }
    }
  }
}

public struct GeneratedCuriosityJson
{
  public string identity;
}