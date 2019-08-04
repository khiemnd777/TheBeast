using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraDeathFeather : MonsterSkill
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
    Transform _coreProjectileRotation;
    [SerializeField]
    Transform[] _featherProjectiles;
    Player2 _player;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;
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
        if (hitPlayer && !hitPlayer.isFendingOff)
        {
            var contactPoint = other.ClosestPointOnBounds (transform.position);
            var dir = other.transform.position - contactPoint;
            dir.Normalize ();
            hitPlayer.OnHit (damage, 9f, dir, contactPoint);
            _slowMotionMonitor.Freeze (.2f, .2f);
            _cameraShake.Shake (.2f, 0.5f);
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
        yield return StartCoroutine (StartRolling ());
        _isStopRolling = false;
        _isRollingMaxSpeed = true;
        StartCoroutine ("KeepRolling");
        StartCoroutine ("KeepRollingProjectiles");
        host.blocked = true;
        WingsInAction (true);
        yield return StartCoroutine (LaunchTheFeathers ());
        headAnimator.Play (closeFacesAnim.name, 0, 0);
        _isStopRolling = true;
        host.blocked = false;
        WingsInAction (false);
        yield return StartCoroutine (StopRolling ());
        _isRollingMaxSpeed = false;
    }

    IEnumerator LaunchTheFeathers ()
    {
        // var t = 0f;
        StartCoroutine (CountdownLaunchTheFeathers ());
        while (!_breakLaunchingTheFeathers)
        {
            foreach (var projectile in _featherProjectiles)
            {
                InstantiateTheFeather (featherPrefab, projectile);
            }
            yield return new WaitForSeconds (.085f);
        }
        _breakLaunchingTheFeathers = false;
    }

    IEnumerator CountdownLaunchTheFeathers ()
    {
        yield return new WaitForSeconds (3f);
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

    IEnumerator KeepRollingProjectiles ()
    {
        while (!_isStopRolling)
        {
            _coreProjectileRotation.Rotate (Vector3.up * Time.fixedDeltaTime * _projectRotationSpeed);
            yield return new WaitForFixedUpdate ();
        }
    }

    void InstantiateTheFeather (MonsterCyloraFeather featherPrefab, Transform projectile)
    {
        var wingDir = (projectile.transform.position - host.transform.position);
        var wingNormal = Vector3.Normalize (wingDir);
        var featherRot = Utilities.RotateByNormal (wingNormal, Vector3.up);
        var feather = Instantiate<MonsterCyloraFeather> (featherPrefab, projectile.transform.position, Quaternion.identity);
        feather.transform.rotation = featherRot;
        Destroy (feather.gameObject, 2f);
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
