using System.Collections;
using Net;
using UnityEngine;

public class Player : NetIdentity, IFieldOfViewVisualizer
{
  public event System.Action onDead;

  public float initLife;

  [System.NonSerialized]
  public bool isFendingOff;

  [System.NonSerialized]
  public Animator animator;
  public NetWeaponController weaponController;
  public MeleeCollider meleeCollider;
  public FieldOfView fieldOfView;

  [System.NonSerialized]
  public float gunWeightIncrement = 1f;

  [SerializeField]
  Blood _playerBlood;

  [SerializeField]
  Blood _playerSlashBlood;

  [SerializeField]
  Blood _playerDead;

  [Space]
  [SerializeField]
  Transform _body;

  [SerializeField]
  NetScore _score;

  [SerializeField]
  FieldOfViewParam _fieldOfViewParam;

  AudioListener _audioListener;
  DotSightController _dotSightController;
  DotSight _dotSight;
  Rigidbody _rigidbody;
  Settings _settings;
  CameraController _cameraController;
  NetRegistrar _netRegistrar;

  [Header("UI")]
  [SerializeField]
  PlayerNameUI _playerNameUI;

  Locker _locker = new Locker();
  public Locker locker { get { return _locker; } }

  protected override void Start()
  {
    base.Start();
    _netRegistrar = FindObjectOfType<NetRegistrar>();
    _rigidbody = GetComponent<Rigidbody>();
    _settings = Settings.instance;
    _audioListener = GetComponent<AudioListener>();
    _audioListener.enabled = false;
    animator = GetComponent<Animator>();
    animator.enabled = false;
    fieldOfView.enabled = false;
    _body.gameObject.SetActive(false);
    if (isServer)
    {
      _body.gameObject.SetActive(true);
      this.maxLife = this.currentLife = this.life = initLife;
    }
    if (isLocal)
    {
      _body.gameObject.SetActive(true);
      fieldOfView.enabled = true;
      StartCoroutine(fieldOfView.FindTargets());
      _audioListener.enabled = true;
      _cameraController = FindObjectOfType<CameraController>();
      Debug.Log($"The player transform {this.transform}");
      _cameraController.SetTarget(this.transform);
      _dotSightController = FindObjectOfType<DotSightController>();
      _dotSightController.InitDotSight();
      _dotSightController.SetPlayer(this);
      _dotSightController.VisibleCursor(false);
      _dotSight = _dotSightController.dotSight;
      this.SetDefaultFieldOfView();

      // Player's local mask is excepted to the self-interactions.
      this.gameObject.layer = LayerMask.NameToLayer("PlayerLocal");

      // Set nickname
      _playerNameUI.SetNickname(netName);

      this.maxLife = this.currentLife = this.life = initLife;

      _locker.RegisterLock("Explosion");
      _locker.RegisterLock("Hitting");

      // Sync the life from server
      onMessageReceived += (eventName, eventMessage) =>
      {
        if (eventName == "object_life")
        {
          var lifeJson = Utility.Deserialize<ObjectLifeJson>(eventMessage);
          currentLife = life = lifeJson.life;
          return;
        }
      };
    }
    if (!isLocal)
    {
      _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }
    if (isClient)
    {
      // Set nickname
      _playerNameUI.SetNickname(netName);

      onMessageReceived += (eventName, eventMessage) =>
      {
        // Sync the hitting from server to the client
        if (eventName == "object_hitted")
        {
          var hittedObjJson = Utility.Deserialize<HittedObjectJson>(eventMessage);
          OnHittingUp(
            hittedObjJson.damagePoint,
            hittedObjJson.freezedTime,
            hittedObjJson.hitbackPoint,
            Utility.PositionArrayToVector3(Vector3.zero, hittedObjJson.impactedPosition),
            Utility.PositionArrayToVector3(Vector3.zero, hittedObjJson.normalizedImpactedPosition),
            hittedObjJson.fromPlayerNetId,
            hittedObjJson.bySlash
          );
          return;
        }
        if (eventName == "player_dead")
        {
          var deadObjJson = Utility.Deserialize<DeadObjectJson>(eventMessage);
          life = deadObjJson.life;
          // Dead effect.
          Blood.BleedOutAtPoint(_playerDead,
            Utility.PositionArrayToVector3(Vector3.zero, deadObjJson.normalizedImpactedPosition),
            Utility.PositionArrayToVector3(Vector3.zero, deadObjJson.impactedPosition)
          );
          Debug.Log("DEAD!");
          _netRegistrar.Disenroll(this);
          if (isLocal)
          {
            if (onDead != null)
            {
              onDead();
            }
          }
          Destroy(gameObject, .1f);
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
      if (currentLife != life && !lifeEnd)
      {
        EmitMessage("object_life", new ObjectLifeJson
        {
          life = life
        });
        currentLife = life;
      }
    }
  }

  public void SetDefaultFieldOfView()
  {
    fieldOfView.SetAngle(_fieldOfViewParam.angle);
    fieldOfView.SetRadius(_fieldOfViewParam.radius);
  }

  [System.Obsolete]
  public void OnHit(float damage, float hitbackForce, Vector3 impactedNormal, Vector3 impactedPoint)
  {
    var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, impactedNormal, hitbackForce);
    _rigidbody.velocity = hitbackVel;
    _locker.Lock("Explosion");
    StartCoroutine(ReleaseLockByExplosion());
  }

  float CalculateDamagePoint(float damagePoint, Vector3 impactedPosition, Vector3 center, Vector3 direction)
  {
    var targetDir = impactedPosition - center;
    var angle = Vector3.Angle(targetDir, direction);
    Debug.Log($"Angle of the impaction {angle}");
    var damagePointRate = 1f;
    if (0f <= angle && angle <= 15f || 165f <= angle && angle <= 180f)
    {
      damagePointRate = 1f;
    }
    else if (16 <= angle && angle <= 90f)
    {
      var baseDamageRate = .9f;
      damagePointRate = baseDamageRate * (16 / angle);
    }
    else if (91f <= angle && angle <= 164f)
    {
      var baseDamageRate = .9f;
      damagePointRate = baseDamageRate * ((180f - 164f) / (180f - angle));
    }
    return damagePoint * damagePointRate;
  }

  public void OnHittingUp(float damagePoint, float freezedTime, float hitbackPoint, Vector3 impactedPosition, Vector3 normalizedImpactedPosition, int fromPlayerNetId, bool bySlash)
  {
    if (isServer)
    {
      var expectedDamage = CalculateDamagePoint(damagePoint, impactedPosition, transform.position, _body.right);
      Debug.Log($"damagePoint: {damagePoint}");
      Debug.Log($"expectedDamage: {expectedDamage}");
      life -= expectedDamage;
      if (lifeEnd)
      {
        // Dead!
        Debug.Log($"{clientId} is dead!");
        EmitMessage("player_dead", new DeadObjectJson
        {
          impactedPosition = Utility.Vector3ToPositionArray(impactedPosition),
          normalizedImpactedPosition = Utility.Vector3ToPositionArray(normalizedImpactedPosition),
          life = life
        });

        // Score for player
        _score.Score(fromPlayerNetId);

        // Disenroll when he's dead
        _netRegistrar.Disenroll(this);
        Destroy(gameObject, .1f);
        return;
      }
      EmitMessage("object_hitted", new HittedObjectJson
      {
        damagePoint = damagePoint,
        freezedTime = freezedTime,
        hitbackPoint = hitbackPoint,
        impactedPosition = Utility.Vector3ToPositionArray(impactedPosition),
        normalizedImpactedPosition = Utility.Vector3ToPositionArray(normalizedImpactedPosition),
        fromPlayerNetId = fromPlayerNetId,
        bySlash = bySlash
      });
    }
    if (isLocal)
    {
      var hitbackVel = Utility.HitbackVelocity(_rigidbody.velocity, normalizedImpactedPosition, hitbackPoint);
      StartCoroutine(HitBack(hitbackVel));
      _locker.Lock("Hitting");
      StartCoroutine(ReleaseLockAfterOn("Hitting", freezedTime));
    }
    if (isClient)
    {
      // Bleed in the client-side.
      if (bySlash)
      {
        if (_playerSlashBlood)
        {
          Blood.BleedOutAtPoint(_playerSlashBlood, -normalizedImpactedPosition, impactedPosition);
        }
      }
      else if (_playerBlood)
      {
        Blood.BleedOutAtPoint(_playerBlood, -normalizedImpactedPosition, impactedPosition);
      }
    }
  }

  IEnumerator HitBack(Vector3 hitbackVel)
  {
    _rigidbody.velocity = hitbackVel;
    yield return new WaitForFixedUpdate();
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

  public void OnTargetEnterFov()
  {
    _body.gameObject.SetActive(true);
    _playerNameUI.Visible(true);
  }

  public void OnTargetLeaveFov()
  {
    _body.gameObject.SetActive(false);
    _playerNameUI.Visible(false);
  }

  void OnDrawGizmos()
  {
    var fov = _fieldOfViewParam;
    var fovTransform = transform;
    var referredAngle = 90f;
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

public struct HittedObjectJson
{
  public float damagePoint;
  public float freezedTime;
  public float hitbackPoint;
  public float[] impactedPosition;
  public float[] normalizedImpactedPosition;
  public int fromPlayerNetId;
  public bool bySlash;
}

public struct DeadObjectJson
{
  public float[] impactedPosition;
  public float[] normalizedImpactedPosition;
  public float life;
}

public struct ObjectLifeJson
{
  public float life;
}