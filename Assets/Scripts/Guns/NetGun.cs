using System.Collections.Generic;
using Net;
using UnityEngine;

public class NetGun : MonoBehaviour
{
  public GunHandType gunHandType;
  public GunWeight weight;
  public bool secondAction;
  public bool auto;
  public bool silent;
  public float thetaProjectileAngle;
  public string netBulletPrefabName;

  [Header("Field Of View")]
  [Range(0, 360)]
  public float fieldOfViewReferredAngle;

  public float fieldOfViewRadius;

  [Range(0, 360)]
  public float fieldOfViewAngle;

  public float fieldOfViewSecondRadius;

  [Range(0, 360)]
  public float fieldOfViewSecondAngle;

  [Header("Shells")]
  public Shell shellPrefab;
  public Transform shellEjection;

  [Header("Others")]
  public float knockbackIndex;
  public RuntimeAnimatorController gunAnimatorController;
  public AnimationClip gunHandTypeAnimation;

  [SerializeField]
  protected CuriousGenerator curiousGenerator;

  public float timeBetweenShoot;
  public NetBullet bulletPrefab;

  [SerializeField]
  protected Transform projectile;

  [SerializeField]
  protected Animator flashAnim;

  [SerializeField]
  protected AudioSource audioSource;

  bool _isHoldTrigger;
  bool _availableHoldTrigger;
  float _timeAvailableHoleTrigger = 1f;

  protected HolderSide holderSide = HolderSide.Right;
  protected Player player;
  protected NetIdentity netIdentity;
  protected Settings settings;

  IDictionary<string, bool> _lockControlList = new Dictionary<string, bool>();

  public virtual void OnHoldTrigger(Vector3 dotSightPoint)
  {
    InstantiateBullet(netBulletPrefabName, dotSightPoint);
  }

  public virtual void HoldTrigger(Vector3 dotSightPoint)
  {
    if (!auto && _isHoldTrigger) return;
    if (!_availableHoldTrigger) return;
    // sound of being at launching bullet
    _timeAvailableHoleTrigger = 0f;
    _availableHoldTrigger = false;

    OnHoldTrigger(dotSightPoint);
    if (OnProjectileLaunched != null)
    {
      OnProjectileLaunched();
    }
    OnTriggerEffect();

    // Generate curiosity
    if (!silent)
    {
      curiousGenerator.Generate(curiousGenerator.curiousIdentity);
    }

    _isHoldTrigger = true;

    // Emit message to trigger the pistol.
    var pistolTriggerEventName = holderSide == HolderSide.Left ? "left_hold_trigger" : "right_hold_trigger";
    netIdentity.EmitMessage(pistolTriggerEventName, new GeneratedCuriosityJson
    {
      identity = curiousGenerator.curiousIdentity
    });

    //  Emit message to generate curiosity.
    if (!silent)
    {
      var curiousGenerateEventName = holderSide == HolderSide.Left ? "left_curious_generate" : "right_curious_generate";
      netIdentity.EmitMessage(curiousGenerateEventName, new GeneratedCuriosityJson
      {
        identity = curiousGenerator.curiousIdentity
      });
    }
  }

  public virtual void ReleaseTrigger()
  {
    _isHoldTrigger = false;
  }

  public virtual void OnSecondAction(Vector3 dotSightPoint)
  {

  }

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
    if (netIdentity.isClient)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    this.player.gunWeightIncrement = 1f;
    Destroy(gameObject);
  }

  float _timeBetweenHoldTrigger;
  public virtual Quaternion CalculateBulletQuaternion()
  {
    // it's late +.1s?
    var angleRandom = Time.time - _timeBetweenHoldTrigger > timeBetweenShoot + Time.deltaTime ? 0 : thetaProjectileAngle;
    var rot = projectile.rotation;
    var rotAngle = rot.eulerAngles;
    var subRot = Quaternion.Euler(rotAngle.x, rotAngle.y + Random.Range(-angleRandom, angleRandom), rotAngle.z);
    _timeBetweenHoldTrigger = Time.time;
    return subRot;
  }

  protected virtual void InstantiateBullet(string netBulletPrefabName, Vector3 dotSightPoint)
  {
    var bulletRot = CalculateBulletQuaternion();
    // Launch the bullet
    NetIdentity.InstantiateLocalAndEverywhere<NetBullet>(netBulletPrefabName, bulletPrefab, projectile.position, bulletRot, (netBullet) =>
    {
      return netBullet.CalculateBulletLifetime(dotSightPoint, projectile.position);
    });
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
    if (netIdentity.isClient)
    {
      netIdentity.onMessageReceived += OnMessageReceived;
    }
  }

  public virtual void Update()
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

  public virtual void FixedUpdate()
  {

  }

  public virtual void OnMessageReceived(string eventName, string message)
  {
    if (eventName == "left_hold_trigger" && holderSide == HolderSide.Left)
    {
      OnTriggerEffect();
    }
    if (eventName == "right_hold_trigger" && holderSide == HolderSide.Right)
    {
      OnTriggerEffect();
    }
    if (eventName == "left_curious_generate" && holderSide == HolderSide.Left)
    {
      var data = Utility.Deserialize<GeneratedCuriosityJson>(message);
      curiousGenerator.Generate(data.identity);
    }
    if (eventName == "right_curious_generate" && holderSide == HolderSide.Right)
    {
      var data = Utility.Deserialize<GeneratedCuriosityJson>(message);
      curiousGenerator.Generate(data.identity);
    }
  }

  public virtual void OnTriggerEffect()
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

  void OnDrawGizmos()
  {
    var fovTransform = transform;
    var referredAngle = fieldOfViewReferredAngle;

    Gizmos.color = Color.white;
    // Gizmos.DrawSphere(fovTransform.position, fieldOfViewAngle);

    var viewAngleA = FieldOfViewUtility.DirectionFromAngle(fovTransform, -fieldOfViewAngle / 2 + referredAngle, true);
    var viewAngleB = FieldOfViewUtility.DirectionFromAngle(fovTransform, fieldOfViewAngle / 2 + referredAngle, true);

    Gizmos.DrawLine(fovTransform.position, fovTransform.position + viewAngleA * fieldOfViewRadius);
    Gizmos.DrawLine(fovTransform.position, fovTransform.position + viewAngleB * fieldOfViewRadius);
  }
}
