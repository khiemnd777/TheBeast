using UnityEngine;

public struct EdgeInfo
{
  public Vector3 pointA;
  public Vector3 pointB;

  public EdgeInfo(Vector3 pointA, Vector3 pointB)
  {
    this.pointA = pointA;
    this.pointB = pointB;
  }

  public static EdgeInfo FindEdge(Transform transform, ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float viewRadius, int edgeResolveIterations, float edgeDstThreshold, float referredAngle, LayerMask obstacleMask)
  {
    var minAngle = minViewCast.angle;
    var maxAngle = maxViewCast.angle;
    var minPoint = Vector3.zero;
    var maxPoint = Vector3.zero;

    for (var i = 0; i < edgeResolveIterations; i++)
    {
      var angle = (minAngle + maxAngle) / 2 + referredAngle;
      var newViewCast = ViewCastInfo.GetViewCast(transform, angle, viewRadius, obstacleMask, false);

      var edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDstThreshold;
      if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
      {
        minAngle = angle;
        minPoint = newViewCast.point;
      }
      else
      {
        maxAngle = angle;
        maxPoint = newViewCast.point;
      }
    }
    return new EdgeInfo(minPoint, maxPoint);
  }
}
