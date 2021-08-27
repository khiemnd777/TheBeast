using UnityEngine;

public class NetKatana : NetMelee
{
  [SerializeField]
  TrailRenderer _trail;
  SlowMotionMonitor _slowMotionMonitor;
  CameraShake _cameraShake;

  public override void Awake()
  {
    base.Awake();
  }

  public override void Start()
  {
    base.Start();
    if (netIdentity.isLocal)
    {
      _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor>();
      _cameraShake = FindObjectOfType<CameraShake>();
    }
    if (netIdentity.isServer)
    {
      _trail.enabled = false;
    }
    player.locker.RegisterLock("Katana");
  }

  public override void OnBeforePlayAnimation()
  {
    base.OnBeforePlayAnimation();
    _trail.enabled = false;
  }

  public override void OnAfterPlayAnimation()
  {
    base.OnAfterPlayAnimation();
    _trail.enabled = false;
  }

  public override void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    base.TakeUpArm(holder, hand, handAnimator);
    playerAnimator.Play(commonStyleAnim.name, 0);
  }

  public override void KeepInCover()
  {
    if (_trail) _trail.enabled = false;
    base.KeepInCover();
  }
}
