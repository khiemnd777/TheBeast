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
    Transform _coreProjectileRotation1;
    [SerializeField]
    Transform[] _featherProjectiles1;
    [Space]
    [SerializeField]
    Transform _coreProjectileRotation2;
    [SerializeField]
    Transform[] _featherProjectiles2;
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
        SetActiveTheWings (true);
        yield return StartCoroutine (StartRolling ());
        _breakLaunchingTheFeathers = false;
        _isStopRolling = false;
        _isRollingMaxSpeed = true;
        StartCoroutine (KeepRolling ());
        StartCoroutine (KeepRollingProjectiles1 ());
        host.blocked = true;
        WingsInAction (true);
        StartCoroutine (LaunchTheFeathersAtProjectileSystem1 ());
        StartCoroutine (LaunchTheFeathersAtProjectileSystem2 ());
        yield return StartCoroutine (CountdownLaunchTheFeathersAtStep1 ());
        yield return StartCoroutine (ScaleRolling ());
        yield return StartCoroutine (LaunchTheFeatherAtStep2 ());
        host.blocked = false;
        yield return new WaitForSeconds (7f);
        headAnimator.Play (closeFacesAnim.name, 0, 0);
        SetActiveTheWings (true);
        _isStopRolling = true;
        WingsInAction (false);
        yield return StartCoroutine (StopRolling ());
        _isRollingMaxSpeed = false;
    }

    void LaunchTheFeathersAtProjectile (Transform[] projectiles)
    {
        foreach (var projectile in projectiles)
        {
            InstantiateTheFeather (featherPrefab, projectile);
        }
    }

    IEnumerator LaunchTheFeathersAtProjectileSystem1 ()
    {
        while (!_breakLaunchingTheFeathers)
        {
            LaunchTheFeathersAtProjectile (_featherProjectiles1);
            yield return new WaitForSeconds (.085f);
        }
    }

    void LaunchTheFeatherAsCircle ()
    {
        LaunchTheFeathersAtProjectile (_featherProjectiles2);
        var angles = _coreProjectileRotation2.rotation.eulerAngles;
        _coreProjectileRotation2.rotation = Quaternion.Euler (new Vector3 (angles.x, angles.y + 11.25f, angles.z));
        LaunchTheFeathersAtProjectile (_featherProjectiles2);
        angles = _coreProjectileRotation2.rotation.eulerAngles;
        _coreProjectileRotation2.rotation = Quaternion.Euler (new Vector3 (angles.x, angles.y + 11.25f, angles.z));
        LaunchTheFeathersAtProjectile (_featherProjectiles2);
        angles = _coreProjectileRotation2.rotation.eulerAngles;
        _coreProjectileRotation2.rotation = Quaternion.Euler (new Vector3 (angles.x, angles.y + 11.25f, angles.z));
        LaunchTheFeathersAtProjectile (_featherProjectiles2);
    }

    IEnumerator LaunchTheFeathersAtProjectileSystem2 ()
    {
        yield return new WaitForSeconds (1f);
        while (!_breakLaunchingTheFeathers)
        {
            LaunchTheFeatherAsCircle ();
            yield return new WaitForSeconds (.8f);
            var angles = _coreProjectileRotation2.rotation.eulerAngles;
            _coreProjectileRotation2.rotation = Quaternion.Euler (new Vector3 (angles.x, angles.y + 5.625f, angles.z));
        }
    }

    IEnumerator LaunchTheFeatherAtStep2 ()
    {
        yield return new WaitForSeconds (1f);
        SetActiveTheWings (false);
        var count = 2;
        do
        {
            LaunchTheFeatherAsCircle ();
            _cameraShake.Shake (.2f, 0.5f);
            yield return new WaitForSeconds (.2f);
            var angles = _coreProjectileRotation2.rotation.eulerAngles;
            _coreProjectileRotation2.rotation = Quaternion.Euler (new Vector3 (angles.x, angles.y + 5.625f, angles.z));
        } while (count-- > 0);
        _slowMotionMonitor.Freeze (.2f, .2f);

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
        yield return new WaitForSeconds (5f);
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
            speedRate = Mathf.Lerp (.75f, 0f, _stopRollingSpeedCurve.Evaluate (t));
            coreScaleRate = Mathf.Lerp (.01f, 1f, _stopRollingSpeedCurve.Evaluate (t));
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed * speedRate);
            _coreRotation.localScale = Vector3.one * coreScaleRate;
            foreach (var wing in _wings)
            {
                wing.transform.localScale = Vector3.one * coreScaleRate;
            }
            yield return null;
        }
    }

    IEnumerator ScaleRolling ()
    {
        var coreScaleRate = 0f;
        var scaleTime = 1.25f;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / scaleTime;
            coreScaleRate = Mathf.Lerp (coreRotationScale, .1f, _stopRollingSpeedCurve.Evaluate (t));
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

    IEnumerator KeepRollingProjectiles1 ()
    {
        while (!_isStopRolling)
        {
            _coreProjectileRotation1.Rotate (Vector3.up * Time.fixedDeltaTime * _projectRotationSpeed);
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
        Destroy (feather.gameObject, 3f);
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
