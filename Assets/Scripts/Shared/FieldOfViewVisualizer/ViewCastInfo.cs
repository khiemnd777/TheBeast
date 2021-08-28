using UnityEngine;

public struct ViewCastInfo
{
  public bool hit;
  public Vector3 point;
  public float distance;
  public float angle;

  public ViewCastInfo (bool hit, Vector3 point, float distance, float angle)
  {
    this.hit = hit;
    this.point = point;
    this.distance = distance;
    this.angle = angle;
  }

  public static ViewCastInfo GetViewCast (Transform transform, float globalAngle, float viewRadius, LayerMask obstacleMask, bool angleIsGlobal)
  {
    var dir = FieldOfViewUtility.DirectionFromAngle (transform, globalAngle, angleIsGlobal);
    RaycastHit hit;

    if (Physics.Raycast (transform.position, dir, out hit, viewRadius, obstacleMask))
    {
      return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
    }
    else
    {
      return new ViewCastInfo (false, transform.position + dir * viewRadius, viewRadius, globalAngle);
    }
  }
}
