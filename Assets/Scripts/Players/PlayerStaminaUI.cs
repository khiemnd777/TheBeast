using UnityEngine;

public class PlayerStaminaUI : MonoBehaviour
{
  [SerializeField]
  Player _player;

  [SerializeField]
  Stamina _stamina;

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
    if (_player.isLocal)
    {
      var normalizedLife = 1f - _stamina.stamina;
      transform.localScale = new Vector3(normalizedLife, transform.localScale.y, transform.localScale.z);
    }
  }
}