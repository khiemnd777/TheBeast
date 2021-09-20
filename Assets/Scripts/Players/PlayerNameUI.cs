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
}