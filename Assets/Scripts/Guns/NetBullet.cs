using Net;
using UnityEngine;

public class NetBullet : NetIdentity
{
  public float timeImpactAtMaxDistance;
  public float damage;
  public float hitback;
  public float freezedTime;
  public float maxDistance;
  public LayerMask layerMask;

  [System.NonSerialized]
  public int playerNetId;

  [SerializeField]
  TrailRenderer _trail;
  float _targetDistance;
  BulletImpactEffect _bulletImpactFx;
  RaycastHit _raycastHit;
  Vector3 _direction;
  float _t;
  bool _isHitOnTarget;

  protected override void Start()
  {
    base.Start();
    _bulletImpactFx = GetComponent<BulletImpactEffect>();
    _direction = transform.rotation * Vector3.right;
  }

  public override void OnCloneMessage(string otherMessage)
  {
    base.OnCloneMessage(otherMessage);
    var data = Utility.Deserialize<NetBulletCloneJson>(otherMessage);
    playerNetId = data.playerNetId;
  }

  protected override void Update()
  {
    base.Update();
    if (Physics.Raycast(transform.position, _direction, out _raycastHit, maxDistance, layerMask))
    {
      _targetDistance = _raycastHit.distance;
      _isHitOnTarget = true;
    }
    else
    {
      _targetDistance = maxDistance;
      _isHitOnTarget = false;
    }
    //
    if (_t <= 1f)
    {
      var timeToImpact = timeImpactAtMaxDistance * _targetDistance / maxDistance;
      _t += Time.deltaTime / timeToImpact;
      // Trail goes straight along direction
      _trail.transform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.right * _targetDistance, _t);
      return;
    }
    if (_isHitOnTarget)
    {
      var impactPoint = _raycastHit.point;
      var hitTransform = _raycastHit.transform;
      if (isServer)
      {
        var hitPlayer = hitTransform.GetComponent<Player>();
        if (hitPlayer)
        {
          var impactedPositionNormalize = hitPlayer.transform.position - impactPoint;
          impactedPositionNormalize.Normalize();
          hitPlayer.OnHittingUp(damage, freezedTime, hitback, impactPoint, impactedPositionNormalize, playerNetId, false);
        }
      }
      // var hitMonster = hitTransform.GetComponent<Monster>();
      // if (hitMonster)
      // {
      //   hitMonster.OnHit(transform, hitback, _raycastHit);
      // }
      ActivateBulletImpactedFx(_raycastHit);
    }
    Destroy(gameObject);
  }

  void ActivateBulletImpactedFx(RaycastHit hit)
  {
    _bulletImpactFx.maxSpeed = 4.5f;
    _bulletImpactFx.lifetime = .125f;
    _bulletImpactFx.Use(hit.point, hit.normal);
  }

  public float CalculateBulletLifetime(Vector3 dotSightPoint, Vector3 projectilePoint)
  {
    var bulletVel = maxDistance / timeImpactAtMaxDistance;
    var bulletLength = Vector3.Distance(dotSightPoint, projectilePoint);
    var bulletLifetime = bulletLength / bulletVel;
    return bulletLifetime + Time.fixedDeltaTime;
  }
}

public struct NetBulletCloneJson
{
  public int playerNetId;
}