using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
  public float radius;

  public Vector3 GetPosition()
  {
    var point = transform.position + Random.insideUnitSphere * radius;
    return new Vector3(point.x, .2f, point.z);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, radius);
  }
}
