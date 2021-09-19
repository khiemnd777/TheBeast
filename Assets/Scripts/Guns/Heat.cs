using System.Collections;
using UnityEngine;

public class Heat : MonoBehaviour
{
  const float HEAT_MAX = 1f;
  const float HEAT_MIN = 0f;
  public float timeHeatingDown;
  public float thetaHeatingUp;
  public float heat;
  float _timeHeatGoingDown;
  bool _overheat;

  [SerializeField]
  NetGun _gun;

  void Start()
  {
    _gun.locker.RegisterLock("Heat");
    _gun.OnProjectileLaunched += OnProjectileLaunched;
  }

  void Update()
  {
    if (_overheat && heat <= HEAT_MIN)
    {
      heat = HEAT_MIN;
      _overheat = false;
      _gun.locker.Unlock("Heat");
    }
    if (heat > HEAT_MIN)
    {
      heat -= Time.deltaTime / timeHeatingDown;
    }
  }

  void OnProjectileLaunched()
  {
    if (!_overheat)
    {
      heat += Time.deltaTime * thetaHeatingUp;
      if (heat >= HEAT_MAX)
      {
        heat = HEAT_MAX;
        _overheat = true;
        _gun.locker.Lock("Heat");
      }
    }
  }
}
