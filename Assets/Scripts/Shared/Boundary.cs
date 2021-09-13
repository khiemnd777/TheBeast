using UnityEngine;

public class Boundary : MonoBehaviour
{
  public BoundingBox boundary;

  void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(boundary.center, boundary.size);
  }
}
