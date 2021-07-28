using UnityEngine;

public static class ObjectExtensions
{
  public static bool IsNotNull(this Object target)
  {
    return target != null && target is Object && !target.Equals(null);
  }
}