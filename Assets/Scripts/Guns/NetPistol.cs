using Net;
using UnityEngine;

public class NetPistol : NetGun
{
  public float timeBetweenShoot;
  public NetBullet bulletPrefab;
  [SerializeField]
  Transform _projectile;
  [SerializeField]
  Animator _flashAnim;
  [SerializeField]
  AudioSource _audioSource;
  bool _isHoldTrigger;
  bool _availableHoldTrigger;
  float _timeAvailableHoleTrigger = 1f;

  public override void Update()
  {
    if (_timeAvailableHoleTrigger < 1f)
    {
      _timeAvailableHoleTrigger += Time.deltaTime / timeBetweenShoot;
    }
    if (_timeAvailableHoleTrigger >= 1f)
    {
      _availableHoldTrigger = true;
    }
  }

  public override void HoldTrigger()
  {
    if (_isHoldTrigger) return;
    if (!_availableHoldTrigger) return;
    // sound of being at launching bullet
    _timeAvailableHoleTrigger = 0f;
    _availableHoldTrigger = false;
    // Launch the bullet
    NetIdentity.InstantiateLocalAndEverywhere<NetBullet>("pistol_bullet", bulletPrefab, _projectile.position, _projectile.rotation);
    if (OnProjectileLaunched != null)
    {
      OnProjectileLaunched();
    }
    _flashAnim.Play("Gun Flash", 0, 0);
    EjectShell();
    // yield return new WaitForSeconds (.02f);
    _audioSource.Play();
    _isHoldTrigger = true;
  }

  public override void ReleaseTrigger()
  {
    _isHoldTrigger = false;
  }
}
