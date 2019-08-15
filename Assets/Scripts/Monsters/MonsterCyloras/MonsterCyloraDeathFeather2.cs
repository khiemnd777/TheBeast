using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraDeathFeather2 : MonsterSkill
{
    public MonsterCylora host;
    public MonsterCyloraFeather featherPrefab;
    public AnimationClip defaultAnim;
    public Animator headAnimator;
    public AnimationClip openFacesAnim;
    public AnimationClip closeFacesAnim;
    public AnimationClip passiveStoppingAnim;
    public AnimationClip passiveStoppingAtFaceAnim;
    public float coreRotationScale;
    public float wingSpeed;
    public float startRollingTime;
    public float stopRollingTime;
    public float idleToDashTime;
    public float dashingVelocity;
    [SerializeField]
    Transform _coreRotation;
    [SerializeField]
    AnimationCurve _startRollingSpeedCurve;
    [SerializeField]
    AnimationCurve _stopRollingSpeedCurve;
    [SerializeField]
    MonsterCyloraWing[] _wings;
    [Space]
    [SerializeField]
    float _projectRotationSpeed;
    [SerializeField]
    Transform _coreProjectileRotation1;
    [SerializeField]
    Transform[] _featherProjectiles1;
    [Space]
    [SerializeField]
    Transform _coreProjectileRotation2;
    [SerializeField]
    Transform[] _featherProjectiles2;
    [Space]
    [SerializeField]
    Transform _coreProjectileRotation3;
    [SerializeField]
    Transform _featherProjectile3;
    Player2 _player;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;
    bool _isStopDashing;
    bool _isStopRolling;
    bool _isRollingMaxSpeed;
    bool _isHitBack;
    float _currentAcceleration;
    bool _breakLaunchingTheFeathers;

    public override void Awake ()
    {
        base.Awake ();
        _player = FindObjectOfType<Player2> ();
        _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
        _cameraShake = FindObjectOfType<CameraShake> ();
        foreach (var wing in _wings)
        {
            wing.onHit += OnWingHit;
        }
    }

    void OnWingHit (MonsterCyloraWing wing, Collider other)
    {
        if (!beExecuting) return;
        if (!wing.weaponEntity.anyAction) return;
        if (!_isRollingMaxSpeed) return;
        if (!other) return;
        var hitPlayer = other.GetComponent<Player2> ();
        if (hitPlayer)
        {
            if (hitPlayer.isFendingOff)
            {
                StopExecutingSkill ();
            }
            else
            {
                var contactPoint = other.ClosestPointOnBounds (transform.position);
                var dir = other.transform.position - contactPoint;
                dir.Normalize ();
                hitPlayer.OnHit (damage, 9f, dir, contactPoint);
                _slowMotionMonitor.Freeze (.2f, .2f);
                _cameraShake.Shake (.2f, 0.5f);
            }
        }
    }

    void WingsInAction (bool anyAction)
    {
        foreach (var wing in _wings)
        {
            wing.weaponEntity.anyAction = anyAction;
        }
    }

    IEnumerator OnBeforeExecuting ()
    {
        host.StopMoving ();
        host.animator.enabled = false;
        _currentAcceleration = host.agent.acceleration;
        yield break;
    }

    IEnumerator OnAfterExecuting ()
    {
        host.agent.acceleration = _currentAcceleration;
        host.animator.enabled = true;
        host.animator.Play (defaultAnim.name, 0, 0);
        // host.KeepLeadingToTarget ();
        host.KeepMoving ();
        yield break;
    }

    public override IEnumerator OnExecuting ()
    {
        SetActiveTheWings (true);
        yield return StartCoroutine (StartRolling ());
        _breakLaunchingTheFeathers = false;
        _isStopRolling = false;
        _isRollingMaxSpeed = true;
        StartCoroutine (KeepRolling ());
        host.blocked = true;
        WingsInAction (true);
        StartCoroutine (LaunchTheFeathersAtProjectileSystem4 ());
        yield return StartCoroutine (CountdownLaunchTheFeathersAtStep1 ());
        yield return new WaitForSeconds (idleToDashTime);
        _isStopDashing = false;
        yield return StartCoroutine (DashToTarget ());
        _isStopDashing = true;
        host.blocked = false;
        headAnimator.Play (closeFacesAnim.name, 0, 0);
        SetActiveTheWings (true);
        _isStopRolling = true;
        WingsInAction (false);
        yield return StartCoroutine (StopRolling ());
        _isRollingMaxSpeed = false;
    }

    IEnumerator DashToTarget ()
    {
        var destPosition = _player.transform.position;
        var startPosition = host.transform.position;
        var distance = Vector3.Distance (_player.transform.position, host.transform.position) + 10f;
        var velocity = dashingVelocity;
        var t = distance / velocity;
        var p = 0f;
        host.agent.acceleration = 2000f;
        while (p <= 1f)
        {
            if (_isStopDashing) yield break;
            p += Time.deltaTime / t;
            // host.transform.position = Vector3.Lerp (startPosition, destPosition, p);
            var direction = _player.transform.position - host.transform.position;
            var normal = Vector3.Normalize (direction);
            var vel = normal * velocity;
            // host.SetVelocity (vel);
            host.agent.velocity = vel;
            // host.KeepMoving(velocity);
            yield return null;
        }
        // host.SetVelocity (Vector3.zero);
        // host.agent.velocity = Vector3.zero;
    }

    IEnumerator LaunchTheFeathersAtProjectileSystem4 ()
    {
        var launch = new System.Action (() =>
        {
            var rotatedAngle = Random.Range (0f, 360f);
            var speed = Random.Range (9f, 14f);
            _coreProjectileRotation3.rotation = Quaternion.Euler (0f, rotatedAngle, 0f);
            InstantiateTheFeather (featherPrefab, _featherProjectile3, speed);
        });
        while (!_breakLaunchingTheFeathers)
        {
            var count = 5;
            while (count-- > 0)
            {
                launch ();
            }
            yield return new WaitForSeconds (.035f);
        }
    }

    void SetActiveTheWings (bool active)
    {
        foreach (var wing in _wings)
        {
            wing.gameObject.SetActive (active);
        }
    }

    IEnumerator CountdownLaunchTheFeathersAtStep1 ()
    {
        yield return new WaitForSeconds (8f);
        _breakLaunchingTheFeathers = true;
    }

    IEnumerator StartRolling ()
    {
        headAnimator.Play (openFacesAnim.name, 0, 0);
        var speedRate = 0f;
        var coreScaleRate = 0f;
        var startTime = Mathf.Max (startRollingTime, openFacesAnim.length);
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / startRollingTime;
            speedRate = Mathf.Lerp (0f, 1f, _startRollingSpeedCurve.Evaluate (t));
            coreScaleRate = Mathf.Lerp (1f, coreRotationScale, _startRollingSpeedCurve.Evaluate (t));
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed * speedRate);
            _coreRotation.localScale = Vector3.one * coreScaleRate;
            yield return null;
        }
    }

    IEnumerator StopRolling ()
    {
        var speedRate = 0f;
        var coreScaleRate = 0f;
        var stopTime = Mathf.Max (stopRollingTime, closeFacesAnim.length);
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / stopTime;
            speedRate = Mathf.Lerp (1f, 0f, _stopRollingSpeedCurve.Evaluate (t));
            coreScaleRate = Mathf.Lerp (coreRotationScale, 1f, _stopRollingSpeedCurve.Evaluate (t));
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed * speedRate);
            _coreRotation.localScale = Vector3.one * coreScaleRate;
            yield return null;
        }
    }

    IEnumerator KeepRolling ()
    {
        while (!_isStopRolling)
        {
            _coreRotation.Rotate (Vector3.back * Time.fixedDeltaTime * wingSpeed);
            yield return null;
        }
    }

    void InstantiateTheFeather (MonsterCyloraFeather featherPrefab, Transform projectile, float speed)
    {
        var wingDir = (projectile.transform.position - host.transform.position);
        var wingNormal = Vector3.Normalize (wingDir);
        var featherRot = Utilities.RotateByNormal (wingNormal, Vector3.up);
        var feather = Instantiate<MonsterCyloraFeather> (featherPrefab, projectile.transform.position, Quaternion.identity);
        feather.damage = damage;
        feather.speed = speed;
        feather.transform.rotation = featherRot;
        Destroy (feather.gameObject, 3f);
    }

    void TurnOffWingColliders ()
    {
        foreach (var wing in _wings)
        {
            wing.TurnOffCollider ();
        }
    }

    void TurnOnWingColliders ()
    {
        foreach (var wing in _wings)
        {
            wing.TurnOnCollider ();
        }
    }

    public override void OnStoppedExecutingSkill ()
    {
        TurnOffWingColliders ();
        skillHandler.isPassiveFendingOff = true;
        host.agent.acceleration = _currentAcceleration;
        host.blocked = false;
        WingsInAction (false);
        host.animator.enabled = true;
        // host.animator.Play (defaultAnim.name, 0, 0);
        StartCoroutine (PassiveStoppingOnFendingOff ());
    }

    IEnumerator PassiveStoppingOnFendingOff ()
    {
        host.animator.Play (passiveStoppingAnim.name, 0, 0);
        headAnimator.Play (passiveStoppingAtFaceAnim.name, 0, 0);
        var direction = _player.transform.position - host.transform.position;
        var normal = Vector3.Normalize (direction);
        host.agent.velocity = -normal * 5f;
        yield return new WaitForSeconds (5f);
        host.animator.Play (defaultAnim.name, 0, 0);
        headAnimator.Play (closeFacesAnim.name, 0, 0);
        host.KeepMoving ();
        skillHandler.isPassiveFendingOff = false;
        TurnOnWingColliders ();
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
