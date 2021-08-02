using UnityEngine;

public static class MathUtility
{
  public static float SineWave (float amplitude, float speed, float t)
  {
    return amplitude * Mathf.Sin (t * speed);
  }
}
