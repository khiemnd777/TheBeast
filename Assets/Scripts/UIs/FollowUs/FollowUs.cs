using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FollowUs : MonoBehaviour
{
  [SerializeField]
  RectTransform _followUI;

  [SerializeField]
  Text _followText;

  bool _show;
  float _currentLerpX = 25f;

  public void Click()
  {
    if (!_show)
    {
      _show = true;
      _followText.text = "Close";
      // StopCoroutine("Hide");
      StartCoroutine("Show");
    }
    else
    {
      _show = false;
      _followText.text = "Follow us";
      // StopCoroutine("Show");
      StartCoroutine("Hide");
    }
  }

  IEnumerator Show()
  {
    _followUI.anchoredPosition = new Vector2(_currentLerpX, 50f);

    var t = 0f;
    while (t <= 1f)
    {
      if(!_show) yield break;
      t += Time.deltaTime / 1f;
      var lerpX = Mathf.Lerp(_currentLerpX, -60f, t);
      _followUI.anchoredPosition = new Vector2(lerpX, 50f);
      _currentLerpX = lerpX;
      yield return null;
    }
  }

  IEnumerator Hide()
  {
    _followUI.anchoredPosition = new Vector2(_currentLerpX, 50f);

    var t = 0f;
    while (t <= 1f)
    {
      if(_show) yield break;
      t += Time.deltaTime / 1f;
      var lerpX = Mathf.Lerp(_currentLerpX, 25f, t);
      _followUI.anchoredPosition = new Vector2(lerpX, 50f);
      _currentLerpX = lerpX;
      yield return null;
    }
  }
}