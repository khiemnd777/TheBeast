using System.Collections;
using UnityEngine;

public class CuriousMark : MonoBehaviour
{
  [SerializeField]
  Transform _mark;
  [SerializeField]
  float _timeMarkPerforming = .5f;

  [SerializeField]
  float _lifetime = 4f;

  void Start()
  {
    StartCoroutine("PerformMark", _timeMarkPerforming);
    Destroy(gameObject, _lifetime);
  }

  IEnumerator PerformMark(float timeMarkPerforming)
  {
    var originScale = Vector3.zero;
    var destScale = Vector3.one;
    var t = 0f;
    while (t <= 1f)
    {
      t += Time.fixedDeltaTime / timeMarkPerforming;
      _mark.localScale = Vector3.Lerp(originScale, destScale, t);
      yield return new WaitForFixedUpdate();
    }
  }
}
