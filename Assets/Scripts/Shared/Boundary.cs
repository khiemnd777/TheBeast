using UnityEngine;

public class Boundary : MonoBehaviour
{
  public BoundingBox boundary;

  void OnDrawGizmos()
  {
    if (boundary.centerTarget)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(boundary.centerTarget.position, boundary.size);
    }
  }
}
