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
      animator = GetComponent<Animator>();
      this.life = 300f;
      this.maxLife = 300f;
      // _footstepSoundFx.volume = sprintVolume;
      _locker.RegisterLock("Explosion");
    }
  }

  public void OnHit(float damage, float hitbackForce, Vector3 impactedNormal, Vector3 impactedPoint)
  {
    var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, impactedNormal, hitbackForce);
    _rigidbody.velocity = hitbackVel;
    _locker.Lock("Explosion");
    StartCoroutine(ReleaseLockByExplosion());
  }

  /// <summary>
  /// Fill health point to player.
  /// </summary>
  /// <param name="hp"></param>
  public void SetHp(float hp)
  {
    this.life = hp;
  }

  /// <summary>
  /// Fill MAX health point to player.
  /// </summary>
  /// <param name="maxHp"></param>
  public void SetMaxHp(float maxHp)
  {
    this.maxLife = maxHp;
  }

  /// <summary>
  /// Add value into MAX health point.
  /// </summary>
  /// <param name="value"></param>
  public void AddMaxHp(float value)
  {
    this.maxLife += value;
  }

  public void AddHp(float hp)
  {
    this.life += hp;
    if (this.life >= this.maxLife)
    {
      this.maxLife = this.life;
    }
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
}
