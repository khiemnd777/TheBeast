using System.Collections;
using UnityEngine;

public class CuriousListener : MonoBehaviour
{
  [System.NonSerialized]
  public string curiousIdentity;

  public float radius;
  public LayerMask targetMask;
  public float delayListeningFootstep;
  public Transform affectedTransform;
  public CuriousMark markPrefab;

  public bool visualizeOnGizmos = false;

  void Start()
  {
    if (!affectedTransform)
    {
      affectedTransform = transform;
    }
  }

  public void Listen()
  {
    StartCoroutine("ListenFootstepWithDelay", delayListeningFootstep);
  }

  IEnumerator ListenFootstepWithDelay(float delay)
  {
    while (true)
    {
      yield return new WaitForSeconds(delay);
      var targetsInRadius = Physics.OverlapSphere(affectedTransform.position, radius, targetMask);
      foreach (var target in targetsInRadius)
      {
        var curiosity = target.GetComponent<Curiosity>();
        if (curiosity)
        {
          if (!curiosity.curiousIdentity.Equals(curiousIdentity))
          {
            Instantiate<CuriousMark>(markPrefab, target.transform.position, Quaternion.identity);
          }
        }
      }
    }
  }

  void OnDrawGizmos()
  {
    if (visualizeOnGizmos)
    {
      // Draw a yellow sphere at the transform's position
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(affectedTransform.position, radius);
    }
  }
}
