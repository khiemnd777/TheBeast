using UnityEngine;

public static class ObjectExtensions
{
  public static bool IsNotNull(this Object target)
  {
    return target != null && target is Object && !target.Equals(null);
  }

  public static bool IsNull(this Object target)
  {
    return !IsNotNull(target);
  }
}