using Net;
using UnityEngine;

public class Stamina : MonoBehaviour
{
  const float STAMINA_MAX = 1f;
  const float STAMINA_MIN = 0f;
  public float timeStaminaDown;
  public float thetaStaminaUp;
  public float stamina;
  float _timeHeatGoingDown;
  bool _overStamina;

  [SerializeField]
  NetPlayerController _playerController;

  void Start()
  {
    _playerController.OnSprint += OnSprint;
    _playerController.sprintLocker.RegisterLock("Stamina");
  }

  void Update()
  {
    if (_overStamina && stamina <= STAMINA_MIN)
    {
      stamina = STAMINA_MIN;
      _overStamina = false;
      _playerController.sprintLocker.Unlock("Stamina");
    }
    if (stamina > STAMINA_MIN)
    {
      stamina -= Time.deltaTime / timeStaminaDown;
    }
  }

  void OnSprint()
  {
    if (!_overStamina)
    {
      stamina += Time.deltaTime / thetaStaminaUp;
      if (stamina >= STAMINA_MAX)
      {
        stamina = STAMINA_MAX;
        _overStamina = true;
        _playerController.sprintLocker.Lock("Stamina");
      }
    }
  }
}
