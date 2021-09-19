using UnityEngine;

public class PlayerLifeUI : MonoBehaviour
{
  [SerializeField]
  Player _player;

  [SerializeField]
  Canvas _canvas;

  void Start()
  {
    if (!_player.isLocal)
    {
      _canvas.gameObject.SetActive(false);
    }
  }

  void Update()
  {
    if (!_player.isServer)
    {
      var normalizedLife = _player.life / _player.maxLife;
      transform.localScale = new Vector3(normalizedLife, transform.localScale.y, transform.localScale.z);
    }
  }
}