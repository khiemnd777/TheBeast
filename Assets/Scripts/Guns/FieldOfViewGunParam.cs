
using UnityEngine;

[System.Serializable]
public struct FieldOfViewParam
{
  public float radius;

  [Range(0, 360)]
  public float angle;
}