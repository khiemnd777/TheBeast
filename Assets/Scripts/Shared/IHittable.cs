using UnityEngine;

public interface IHittable
{
  /// <summary>
  /// Hit.
  /// </summary>
  void Hit (Transform hitFrom, float damage);
}
