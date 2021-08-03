using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

  protected override void Awake()
  {
    base.Awake();
  }

  protected override void Start()
  {
    base.Start();
    _rigidbody = GetComponent<Rigidbody>();
    _settings = Settings.instance;
    if (isLocal)
    {
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
      _footstepSoundFx.volume = sprintVolume;
      RegisterLock("Explosion");
    }
  }

  protected override void Update()
  {
    base.Update();
    Rotate2();
    if (IsLocked())
    {
      _speed = 0;
      return;
    }
    var x = Input.GetAxisRaw("Horizontal");
    var y = Input.GetAxisRaw("Vertical");
    _isMoving = x != 0 || y != 0;
    _direction = Utility.AlterVector3(_direction, x, y);
    // Sprint by default
    _speed = sprintSpeed;
    _footstepSoundFx.volume = sprintVolume;
    // Walk
    if (Input.GetKey(KeyCode.LeftShift))
    {
      _speed = walkSpeed;
      _footstepSoundFx.volume = walkVolume;
    }
    if (_isMoving)
    {
      // foot rotation
      _foots.rotation = Quaternion.LookRotation(Vector3.up, _direction);
      _isStopping = false;
      _timeFootOnGround += Time.deltaTime / (_settings.playerFootOnGroundDelta / _speed);
      if (_timeFootOnGround >= 1)
      {
        _isLeftFoot = !_isLeftFoot;
        _timeFootOnGround = 0f;
      }
    }
    else if (!_isStopping)
    {
      _isLeftFoot = !_isLeftFoot;
      _timeFootOnGround = 0f;
      _isStopping = true;
    }
  }

  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    // if (IsLocked ())
    // {
    // 	Debug.Log(1);
    // 	return;
    // }
    _rigidbody.velocity = _direction * _speed;
  }

  void Rotate2()
  {
    if (isLocal)
    {
      if (_dotSightController)
      {
        var normal = _dotSight.NormalizeFromPoint(transform.position);
        var destRotation = Utility.RotateByNormal(normal, Vector3.up);
        body.rotation = Quaternion.RotateTowards(body.rotation, destRotation, Time.deltaTime * 630f);
      }

    }
  }

  public void OnHit(float damage, float hitbackForce, Vector3 impactedNormal, Vector3 impactedPoint)
  {
    var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, impactedNormal, hitbackForce);
    _rigidbody.velocity = hitbackVel;
    Lock("Explosion");
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
    Unlock("Explosion");
  }

  public void RegisterLock(string name)
  {
    if (_lockControlList.ContainsKey(name)) return;
    _lockControlList.Add(name, false);
  }

  public void Lock(string name)
  {
    if (!_lockControlList.ContainsKey(name)) return;
    _lockControlList[name] = true;
  }

  public void Unlock(string name)
  {
    if (!_lockControlList.ContainsKey(name)) return;
    _lockControlList[name] = false;
  }

  public bool IsLocked()
  {
    return _lockControlList.Values.Any(locked => locked);
  }
}
