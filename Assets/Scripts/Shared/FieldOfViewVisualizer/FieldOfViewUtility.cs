using UnityEngine;

public static class FieldOfViewUtility
{
  public static Vector3 DirectionFromAngle(Transform transform, float angleInDegrees, bool angleIsGlobal, FieldOfViewDirection direction = FieldOfViewDirection.up)
  {
    if (!angleIsGlobal)
    {
      angleInDegrees += transform.eulerAngles.y;
    }
    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
  }
}
