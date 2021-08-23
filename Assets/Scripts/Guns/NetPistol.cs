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

  public override void Start()
  {
    base.Start();
    if (netIdentity.isClient)
    {
      netIdentity.onMessageReceived += OnMessageReceived;
    }
  }

  public override void Update()
  {
    base.Update();
    if (_timeAvailableHoleTrigger < 1f)
    {
      _timeAvailableHoleTrigger += Time.deltaTime / timeBetweenShoot;
    }
    if (_timeAvailableHoleTrigger >= 1f)
    {
      _availableHoldTrigger = true;
    }
  }

  public override void HoldTrigger(Vector3 dotSightPoint)
  {
    if (_isHoldTrigger) return;
    if (!_availableHoldTrigger) return;
    // sound of being at launching bullet
    _timeAvailableHoleTrigger = 0f;
    _availableHoldTrigger = false;
    // Launch the bullet
    NetIdentity.InstantiateLocalAndEverywhere<NetBullet>("pistol_bullet", bulletPrefab, _projectile.position, _projectile.rotation, (netBullet) => {
      var bulletVel = netBullet.maxDistance / netBullet.timeImpactAtMaxDistance;
      var bulletLength = Vector3.Distance(dotSightPoint, _projectile.position);
      var bulletLifetime = bulletLength / bulletVel;
      return bulletLifetime;
    });
    DoesTriggerEffect();
    _isHoldTrigger = true;
    // Emit message to trigger the pistol.
    var eventName = holderSide == HolderSide.Left ? "left_pistol_trigger" : "right_pistol_trigger";
    netIdentity.EmitMessage(eventName, null);
  }

  void OnMessageReceived(string eventName, string message)
  {
    if (eventName == "left_pistol_trigger" && holderSide == HolderSide.Left)
    {
      DoesTriggerEffect();
    }
    if (eventName == "right_pistol_trigger" && holderSide == HolderSide.Right)
    {
      DoesTriggerEffect();
    }
  }

  public override void KeepInCover()
  {
    if (netIdentity.isClient)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    base.KeepInCover();
  }

  void DoesTriggerEffect()
  {
    if (OnProjectileLaunched != null)
    {
      OnProjectileLaunched();
    }
    _flashAnim.Play("Gun Flash", 0, 0);
    EjectShell();
    _audioSource.Play();
  }

  public override void ReleaseTrigger()
  {
    _isHoldTrigger = false;
  }
}
