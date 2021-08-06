using UnityEngine;

public class Heat : MonoBehaviour
{
  public float heatMax = 70f;
  public float heatMin = 20f;
  public float heatStepUp;
  public float heatStepDown;
  public float timeHeatDown;
  public float heat;
  float _timeHeatGoingDown;

  Gun _gun;

  void Awake()
  {
    _gun = GetComponent<Gun>();
    _gun.locker.RegisterLock("Heat");
    _gun.OnProjectileLaunched += OnProjectileLaunched;
  }

  void Update()
  {
    HeatGoingDown();
  }

  void HeatGoingDown()
  {
    // var theta = 
    _timeHeatGoingDown += Time.deltaTime / timeHeatDown;
    if (_timeHeatGoingDown >= 1f)
    {
      heat -= heatStepDown;
      _timeHeatGoingDown = 0;
    }
    if (heat <= heatMin)
    {
      _gun.locker.Unlock("Heat");
    }
    if (heat <= 0)
    {
      heat = 0;
    }
  }

  void HeatUp()
  {
    heat += heatStepUp;
  }

  bool CheckExceedHeatMax()
  {
    return heat >= heatMax;
  }

  void OnProjectileLaunched()
  {
    HeatUp();
    if (CheckExceedHeatMax())
    {
      _gun.locker.Lock("Heat");
    }
  }
}
