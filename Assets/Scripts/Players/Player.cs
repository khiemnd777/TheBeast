using System.Collections;
using System.Collections.Generic;
using Net;
using UnityEngine;

public class Player : NetIdentity
{
  public float sprintSpeed = 1f;
  public float walkSpeed = .5f;
  public float sprintVolume = .09f;
  public float walkVolume = .01f;
  [System.NonSerialized]
  public bool isFendingOff;
  [SerializeField]
  public Transform body;
  [System.NonSerialized]
  public Animator animator;
  public WeaponController weaponController;
  [SerializeField]
  Transform _foots;
  [SerializeField]
  Transform _leftFoot;
  [SerializeField]
  Transform _rightFoot;
  [Space]
  [SerializeField]
  AudioSource _footstepSoundFx;
  AudioListener _audioListener;
  IDictionary<string, bool> _lockControlList = new Dictionary<string, bool>();
  DotSightController _dotSightController;
  DotSight _dotSight;
  Vector3 _direction;
  Rigidbody _rigidbody;
  bool _isLeftFoot;
  bool _isMoving;
  bool _isStopping = true;
  float _timeFootOnGround;
  float _speed;
  Settings _settings;
  CameraController _cameraController;

  Locker _locker = new Locker();
  public Locker locker { get { return _locker; } }

  protected override void Start()
  {
    base.Start();
    _rigidbody = GetComponent<Rigidbody>();
    _settings = Settings.instance;
    _audioListener = GetComponent<AudioListener>();
    _audioListener.enabled = false;
    animator = GetComponent<Animator>();
    animator.enabled = false;
    if (isLocal)
    {
      _audioListener.enabled = true;
      _cameraController = FindObjectOfType<CameraController>();
      _cameraController.SetTarget(this.transform);
      _dotSightController = FindObjectOfType<DotSightController>();
      _dotSightController.InitDotSight();
      _dotSightController.SetPlayer(this);
      _dotSightController.VisibleCursor(false);
      _dotSight = _dotSightController.dotSight;
      this.maxLife = this.currentLife = this.life = 300f;
      // _footstepSoundFx.volume = sprintVolume;
      _locker.RegisterLock("Explosion");
      _locker.RegisterLock("Hitting");
      // Sync the life from server
      onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "object_life")
        {
          var lifeJson = Utility.Deserialize<ObjectLifeJson>(eventMessage);
          currentLife = life = lifeJson.life;
        }
      };
    }
    if (!isServer)
    {
      // Sync the hitting from server to the client
      onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "object_hitted")
        {
          var hittedObjJson = Utility.Deserialize<HittedObjectJson>(eventMessage);
          OnHittingUp(
            hittedObjJson.damagePoint,
            hittedObjJson.freezedTime,
            hittedObjJson.hitbackPoint,
            Utility.PositionArrayToVector3(Vector3.zero, hittedObjJson.normalizedImpactedPosition)
          );
        }
      };
    }
  }

  protected override void Update()
  {
    base.Update();
    if (isServer)
    {
      // Sync the life point to the local
      if (currentLife != life)
      {
        EmitMessage("object_life", new ObjectLifeJson
        {
          life = life
        });
        currentLife = life;
      }
    }
  }

  [System.Obsolete]
  public void OnHit(float damage, float hitbackForce, Vector3 impactedNormal, Vector3 impactedPoint)
  {
    var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, impactedNormal, hitbackForce);
    _rigidbody.velocity = hitbackVel;
    _locker.Lock("Explosion");
    StartCoroutine(ReleaseLockByExplosion());
  }

  public void OnHittingUp(float damagePoint, float freezedTime, float hitbackPoint, Vector3 normalizedImpactedPosition)
  {
    if (isServer)
    {
      life -= damagePoint;
      if (lifeEnd)
      {
        // Dead!
      }
      else
      {
        var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, normalizedImpactedPosition, hitbackPoint);
        _rigidbody.velocity = hitbackVel;
        EmitMessage("object_hitted", new HittedObjectJson
        {
          damagePoint = damagePoint,
          freezedTime = freezedTime,
          hitbackPoint = hitbackPoint,
          normalizedImpactedPosition = Utility.Vector3ToPositionArray(normalizedImpactedPosition)
        });
      }
    }
    if (isClient)
    {
      var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, normalizedImpactedPosition, hitbackPoint);
      _rigidbody.velocity = hitbackVel;
      _locker.Lock("Hitting");
      StartCoroutine(ReleaseLockAfterOn("Hitting", freezedTime));
    }
  }

  /// <summary>
  /// Fill health point to player.
  /// </summary>
  /// <param name="hp"></param>
  [System.Obsolete]
  public void SetHp(float hp)
  {
    this.life = hp;
  }

  /// <summary>
  /// Fill MAX health point to player.
  /// </summary>
  /// <param name="maxHp"></param>
  [System.Obsolete]
  public void SetMaxHp(float maxHp)
  {
    this.maxLife = maxHp;
  }

  public void OnFendingOff(float knockbackForce, Vector3 impactedNormal, Vector3 impactedPoint)
  {
    var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, impactedNormal, knockbackForce);
    _rigidbody.velocity = hitbackVel;
    isFendingOff = true;
    StartCoroutine(SetFendingOffStatusOff());
  }

  IEnumerator SetFendingOffStatusOff()
  {
    yield return new WaitForSeconds(_settings.defaultFendingOffStatusOffTime);
    isFendingOff = false;
  }

  public IEnumerator ReleaseLockByExplosion()
  {
    yield return new WaitForSeconds(_settings.defaultReleaseLockExplosionTime);
    _locker.Unlock("Explosion");
  }

  public IEnumerator ReleaseLockAfterOn(string lockName, float freezedTime)
  {
    yield return new WaitForSeconds(freezedTime);
    _locker.Unlock(lockName);
  }
}

public struct HittedObjectJson
{
  public float damagePoint;
  public float freezedTime;
  public float hitbackPoint;
  public float[] normalizedImpactedPosition;
}

public struct ObjectLifeJson
{
  public float life;
}