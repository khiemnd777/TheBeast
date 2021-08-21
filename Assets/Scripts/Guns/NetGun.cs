using System.Collections.Generic;
using Net;
using UnityEngine;

public abstract class NetGun : MonoBehaviour
{
  public GunHandType gunHandType;
  public float knockbackIndex;
  public RuntimeAnimatorController gunAnimatorController;
  public AnimationClip gunHandTypeAnimation;
  [Header("Shells")]
  public Shell shellPrefab;
  public Transform shellEjection;

  protected HolderSide holderSide = HolderSide.Right;
  protected Player player;
  protected NetIdentity netIdentity;
  protected Settings settings;

  IDictionary<string, bool> _lockControlList = new Dictionary<string, bool>();

  public abstract void HoldTrigger();
  public abstract void ReleaseTrigger();
  public System.Action OnProjectileLaunched;

  Locker _locker = new Locker();
  public Locker locker { get { return _locker; } }

  public virtual void TakeUpArm()
  {
    if (player.animator)
    {
      player.animator.enabled = false;
      player.animator.runtimeAnimatorController = gunAnimatorController;
      player.animator.Play(gunHandTypeAnimation.name, 0);
    }
  }

  public virtual void KeepInCover()
  {
    Destroy(gameObject);
  }

  public void EjectShell()
  {
    // eject shells.
    Instantiate(shellPrefab, shellEjection.position, shellEjection.rotation);
  }

  public virtual void Awake()
  {
  }

  public virtual void Start()
  {
    settings = Settings.instance;
  }

  public virtual void Update()
  {

  }

  public virtual void FixedUpdate()
  {

  }

  public void SetPlayer(Player player)
  {
    this.player = player;
  }

  public void SetHolderSide(HolderSide holderSide)
  {
    this.holderSide = holderSide;
  }

  public void SetNetIdentity(NetIdentity netIdentity)
  {
    this.netIdentity = netIdentity;
  }
}
