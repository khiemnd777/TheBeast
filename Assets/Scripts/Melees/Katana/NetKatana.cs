using UnityEngine;

public class NetKatana : NetMelee
{
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
    player.locker.RegisterLock("Katana");
  }

  public override void OnBeforePlayAnimation()
  {
    base.OnBeforePlayAnimation();
  }

  public override void OnAfterPlayAnimation()
  {
    base.OnAfterPlayAnimation();
  }

  public override void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    base.TakeUpArm(holder, hand, handAnimator);
    playerAnimator.Play(commonStyleAnim.name, 0);
    if (netIdentity.isLocal)
    {
      player.SetDefaultFieldOfView();
    }
  }

  public override void KeepInCover()
  {
    base.KeepInCover();
  }
}
