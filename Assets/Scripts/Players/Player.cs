using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Net;
using UnityEngine;

public class Player : NetIdentity, IFieldOfViewVisualizer, IPicker
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
  DroppedGunController _droppedGunController;

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

  HeartGenerator _heartGenerator;

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
    _body.gameObject.SetActive(!_settings.enableFieldOfView);
    if (isServer)
    {
      _body.gameObject.SetActive(true);
      this.maxLife = this.currentLife = this.life = initLife;
      _heartGenerator = FindObjectOfType<HeartGenerator>();
    }
    if (isLocal)
    {
      _body.gameObject.SetActive(true);
      if (_settings.enableFieldOfView)
      {
        fieldOfView.enabled = true;
        StartCoroutine(fieldOfView.FindTargets());
      }
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
          // Script of destroy
          Debug.Log("You are destroyed!");
          _netRegistrar.Disenroll(this);
          if (isLocal)
          {
            _dotSightController.VisibleCursor(true);
            _dotSightController.DestroyDotSight();
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

  object _playerDeadLockObj = new object();
  bool _playerDeadLocked = false;

  void DoPlayerDead(Vector3 impactedPosition, Vector3 normalizedImpactedPosition, int fromPlayerNetId)
  {
    lock (_playerDeadLockObj)
    {
      if (_playerDeadLocked) return;
      _playerDeadLocked = true;
      // Dead!
      Debug.Log($"{clientId} is dead!");
      EmitMessage("player_dead", new DeadObjectJson
      {
        impactedPosition = Utility.Vector3ToPositionArray(impactedPosition),
        normalizedImpactedPosition = Utility.Vector3ToPositionArray(normalizedImpactedPosition),
        life = life
      });

      // Score for player
      var fromPlayer = (Player)NetObjectList.instance.Find(fromPlayerNetId);
      if (fromPlayer)
      {
        var netScore = fromPlayer.GetComponent<NetScore>();
        if (netScore)
        {
          netScore.ServerScore(fromPlayerNetId, fromPlayer.clientId);
        }
      }

      // Generate heart
      _heartGenerator.Generate(transform.position, Quaternion.identity, 1.25f);

      // Drop the gun
      _droppedGunController.Drop(transform.position, Quaternion.identity, 1.25f);

      // Disenroll when he's dead
      _netRegistrar.Disenroll(this);
      Destroy(gameObject, .1f);
    }
  }

  public void OnHittingUp(float damagePoint, float freezedTime, float hitbackPoint, Vector3 impactedPosition, Vector3 normalizedImpactedPosition, int fromPlayerNetId, bool bySlash)
  {
    if (isServer)
    {
      var expectedDamage = CalculateDamagePoint(damagePoint, impactedPosition, transform.position, _body.right);
      life -= expectedDamage;
      if (lifeEnd)
      {
        // Dead!
        DoPlayerDead(impactedPosition, normalizedImpactedPosition, fromPlayerNetId);
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

  public void AddHp(float hp)
  {
    this.life = this.life + hp >= this.maxLife ? this.maxLife : this.life + hp;
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

  #region IFieldOfViewVisualizer
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
  #endregion

  #region IPicker
  object droppedItemLock = new object();
  List<DroppedItem> _droppedItems = new List<DroppedItem>();
  public List<DroppedItem> droppedItems => _droppedItems;

  public void PickUp()
  {
    lock (droppedItemLock)
    {
      if (_droppedItems.Any())
      {
        foreach (var droppedItem in _droppedItems)
        {
          if (droppedItem)
          {
            droppedItem.PickUp(this);
          }
        }
        // Remove item after picked up
        _droppedItems = _droppedItems.Where(x => x).ToList();
      }
    }
  }

  public void PickUp(DroppedItem droppedItem)
  {
    if (droppedItem)
    {
      droppedItem.PickUp(this);
      lock (droppedItemLock)
      {
        if (_droppedItems.Any())
        {
          // Remove item after picked up
          _droppedItems = _droppedItems.Where(x => x).ToList();
        }
      }
    }
  }

  public void AddDroppedItem(DroppedItem item)
  {
    lock (droppedItemLock)
    {
      if (!_droppedItems.Any(x => x.id == item.id))
      {
        _droppedItems.Add(item);
        Debug.Log($"Add dropped item {item.name}");
      }
    }
  }

  public void RemoveDroppedItem(DroppedItem item)
  {
    lock (droppedItemLock)
    {
      if (_droppedItems.Any(x => x.id == item.id))
      {
        _droppedItems.Remove(item);
        Debug.Log($"Remove dropped item {item.name}");
      }
    }
  }
  #endregion

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