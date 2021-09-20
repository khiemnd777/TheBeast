using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
  [SerializeField]
  Text _nicknameText;

  [SerializeField]
  Player _player;

  [SerializeField]
  Canvas _canvas;

  string _nickname;

  void Start()
  {
    if (_player.isServer)
    {
      _canvas.gameObject.SetActive(false);
    }
    if (_player.isLocal)
    {
      StartCoroutine(FadingOff(3, 4));
    }
  }

  public void SetNickname(string nickname)
  {
    _nicknameText.text = nickname;
  }

  public void Visible(bool visible)
  {
    if (!_player.isServer)
    {
      _canvas.gameObject.SetActive(visible);
    }
  }

  IEnumerator FadingOff(float time, float delay = 0f)
  {
    yield return new WaitForSeconds(delay);
    var t = 0f;
    var color = _nicknameText.color;
    var alpha = color.a;
    while (t <= 1f)
    {
      t += Time.deltaTime / time;
      var a = Mathf.Lerp(alpha, 0f, t);
      color.a = a;
      _nicknameText.color = color;
      yield return null;
    }
  }
}
