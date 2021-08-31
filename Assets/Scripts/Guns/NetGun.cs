using System.Collections.Generic;
using Net;
using UnityEngine;

public abstract class NetGun : MonoBehaviour
{
  public GunHandType gunHandType;
  public GunWeight weight;
  public float knockbackIndex;
  public RuntimeAnimatorController gunAnimatorController;
  public AnimationClip gunHandTypeAnimation;
  [Header("Shells")]
  public Shell shellPrefab;
  public Transform shellEjection;

  [Header("Others")]
  [SerializeField]
  protected CuriousGenerator curiousGenerator;

  protected HolderSide holderSide = HolderSide.Right;
  protected Player player;
  protected NetIdentity netIdentity;
  protected Settings settings;

  IDictionary<string, bool> _lockControlList = new Dictionary<string, bool>();

  public abstract void HoldTrigger(Vector3 dotSightPoint);
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
    this.player.gunWeightIncrement = 1f;
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

  public void SetGunWeightToPlayer(Player player)
  {
    switch (weight)
    {
      case GunWeight.Light:
        {
          player.gunWeightIncrement = .9f;
        }
        break;
      case GunWeight.Medium:
        {
          player.gunWeightIncrement = .8f;
        }
        break;
      case GunWeight.Heavy:
        {
          player.gunWeightIncrement = .5f;
        }
        break;
      case GunWeight.VeryHeavy:
        {
          player.gunWeightIncrement = .25f;
        }
        break;
      case GunWeight.HandFree:
      default:
        {
          player.gunWeightIncrement = 1;
        }
        break;
    }
  }

  public void SetHolderSide(HolderSide holderSide)
  {
    this.holderSide = holderSide;
  }

  public void SetNetIdentity(NetIdentity netIdentity)
  {
    this.netIdentity = netIdentity;
  }

  public virtual void OnAfterTakenUpArm()
  {
    if (curiousGenerator)
    {
      curiousGenerator.curiousIdentity = netIdentity.clientId;
    }
  }
}
