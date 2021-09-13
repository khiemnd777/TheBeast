
using UnityEngine;

[System.Serializable]
public struct FieldOfViewGunParam
{
  public float radius;

  [Range(0, 360)]
  public float angle;
}