using System.Collections;
using UnityEngine;

public class Footstep : MonoBehaviour
{
  public float radius;
  public float lifetime = 4f;
  public SphereCollider footstepCollider;

  [Space]
  [SerializeField]
  Transform _mark;
  [SerializeField]
  float _timeMarkPerforming = .5f;

  void Start()
  {
    if (footstepCollider)
    {
      footstepCollider.radius = radius;
    }
    StartCoroutine("PerformMark", _timeMarkPerforming);
    Destroy(gameObject, lifetime);
  }

  IEnumerator PerformMark(float timeMarkPerforming)
  {
    var originScale = new Vector3(1f, 0f, 1f);
    var destScale = new Vector3(1f, 1f, 1f);
    var t = Time.fixedDeltaTime / timeMarkPerforming;
    while (t <= 1f)
    {
      _mark.localScale = Vector3.Lerp(originScale, destScale, t);
      yield return new WaitForFixedUpdate();
    }
  }
}
