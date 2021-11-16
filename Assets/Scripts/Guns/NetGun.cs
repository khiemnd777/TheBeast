using System.Collections.Generic;
using Net;
using UnityEngine;

public class NetGun : MonoBehaviour
{
  public string registeredName;
  public GunHandType gunHandType;
  public GunWeight weight;
  public bool secondAction;
  public bool auto;
  public bool silent;
  public float thetaProjectileAngle;
  public string netBulletPrefabName;
  public DroppedGun droppedGun;

  [Header("Field Of View")]
  [Range(0, 360)]
  public float fieldOfViewReferredAngle;
  public FieldOfViewParam[] fieldOfViews;

  [SerializeField]
  int _fieldOfViewIndex = 0;

  [Header("Shells")]
  public Shell shellPrefab;
  public Transform shellEjection;

  [Header("Others")]
  [SerializeField]
  protected SpriteRenderer ui;
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

  protected HeatUI heatUI;

  bool _isHoldTrigger;
  bool _availableHoldTrigger;
  float _timeAvailableHoleTrigger = 1f;

  protected HolderSide holderSide = HolderSide.Right;
  protected Player player;

  [System.NonSerialized]
  public NetIdentity netIdentity;

  protected DotSightController dotSightController;
  protected DotSight dotSight;
  protected CameraController cameraController;
  protected Settings settings;

  IDictionary<string, bool> _lockControlList = new Dictionary<string, bool>();

  public virtual void Init()
  {
    settings = Settings.instance;
    if (netIdentity.isClient && !netIdentity.isLocal)
    {
      netIdentity.onMessageReceived += OnMessageReceived;
    }
    if (netIdentity.isLocal)
    {
      dotSightController = FindObjectOfType<DotSightController>();
      if (dotSightController)
      {
        dotSight = dotSightController.dotSight;
      }
      cameraController = FindObjectOfType<CameraController>();
    }
  }

  public virtual void OnHoldTrigger()
  {
    var dotSightPoint = dotSight.GetCurrentPoint();
    InstantiateBullet(netBulletPrefabName, dotSightPoint);
  }

  public virtual void HoldTrigger()
  {
    if (!auto && _isHoldTrigger) return;
    if (!_availableHoldTrigger) return;
    // sound of being at launching bullet
    _timeAvailableHoleTrigger = 0f;
    _availableHoldTrigger = false;
    OnHoldTrigger();
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
    var holdTriggerEventName = holderSide == HolderSide.Left ? "left_hold_trigger" : "right_hold_trigger";
    netIdentity.EmitMessage(holdTriggerEventName, null);

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

  public virtual void OnSecondAction()
  {

  }

  public System.Action OnProjectileLaunched;

  Locker _locker = new Locker();
  public Locker locker { get { return _locker; } }

  public virtual void TakeUpArm()
  {
    this.ui.gameObject.SetActive(true);
    this.enabled = true;
    if (player.animator)
    {
      player.animator.enabled = false;
      player.animator.runtimeAnimatorController = gunAnimatorController;
      player.animator.Play(gunHandTypeAnimation.name, 0);
    }
  }

  public virtual void KeepInCover()
  {
    if (netIdentity.isClient && !netIdentity.isLocal)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    this.player.gunWeightIncrement = 1f;
    this.ui.gameObject.SetActive(false);
    this.enabled = false;
  }

  public virtual void ChangeGun()
  {
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
    var bullet = NetIdentity.InstantiateLocalAndEverywhere<NetBullet>(netBulletPrefabName, bulletPrefab, projectile.position, bulletRot, (netBullet) =>
    {
      return netBullet.CalculateBulletLifetime(dotSightPoint, projectile.position);
    }, new NetBulletCloneJson
    {
      playerNetId = player.id
    });
    bullet.playerNetId = player.id;
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
      // Debug.Log(eventName);
      OnTriggerEffect();
    }
    if (eventName == "right_hold_trigger" && holderSide == HolderSide.Right)
    {
      // Debug.Log(eventName);
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

  public void SetHeatUI(HeatUI heatUI)
  {
    this.heatUI = heatUI;
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
          player.gunWeightIncrement = .7f;
        }
        break;
      case GunWeight.VeryHeavy:
        {
          player.gunWeightIncrement = .6f;
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

  public virtual int SwitchFieldOfViewIndex()
  {
    ++_fieldOfViewIndex;
    if (_fieldOfViewIndex >= fieldOfViews.Length)
    {
      _fieldOfViewIndex = 0;
    }
    return _fieldOfViewIndex;
  }

  public FieldOfViewParam GetFieldOfView()
  {
    return GetFieldOfView(_fieldOfViewIndex);
  }

  public FieldOfViewParam GetFieldOfView(int fovIndex)
  {
    if (
      fieldOfViews != null
      && fieldOfViews.Length > 0
      && fovIndex < fieldOfViews.Length
    )
    {
      return fieldOfViews[fovIndex];
    }
    return new FieldOfViewParam();
  }

  void OnDrawGizmos()
  {
    if (
      fieldOfViews != null
    )
    {
      var fov = GetFieldOfView(_fieldOfViewIndex);
      var fovTransform = transform;
      var referredAngle = fieldOfViewReferredAngle;
      var radius = fov.radius;
      var angle = fov.angle;
      Gizmos.color = Color.white;
      Gizmos.DrawWireSphere(fovTransform.position, radius);

      var viewAngleA = FieldOfViewUtility.DirectionFromAngle(fovTransform, -angle / 2 + referredAngle, true);
      var viewAngleB = FieldOfViewUtility.DirectionFromAngle(fovTransform, angle / 2 + referredAngle, true);

      Gizmos.DrawLine(fovTransform.position, fovTransform.position + viewAngleA * radius);
      Gizmos.DrawLine(fovTransform.position, fovTransform.position + viewAngleB * radius);
    }
  }
}
