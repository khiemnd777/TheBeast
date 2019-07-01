using System.Collections;
using UnityEngine;

public class MonsterCyloraDeathWheel : MonsterSkill
{
	public MonsterCylora host;
	public AnimationClip defaultAnim;
	public Animator headAnimator;
	public AnimationClip openFacesAnim;
	public AnimationClip closeFacesAnim;
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
	Player2 _player;
	SlowMotionMonitor _slowMotionMonitor;
	CameraShake _cameraShake;
	bool _isStopRolling;
	bool _isRollingMaxSpeed;
	bool _isStopDashing;
	bool _isHitBack;

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
			_cameraShake.Shake(.2f, 0.5f);
		}
	}

	void WingsInAction (bool anyAction)
	{
		foreach (var wing in _wings)
		{
			wing.weaponEntity.anyAction = anyAction;
		}
	}

	public override IEnumerator OnExecuting ()
	{
		yield return StartCoroutine (StartRolling ());
		_isStopRolling = false;
		_isRollingMaxSpeed = true;
		StartCoroutine (KeepRolling ());
		yield return new WaitForSeconds (idleToDashTime);
		_isStopDashing = false;
		host.blocked = true;
		WingsInAction (true);
		yield return StartCoroutine (DashToTarget ());
		_isStopDashing = true;
		headAnimator.Play (closeFacesAnim.name, 0, 0);
		_isStopRolling = true;
		host.blocked = false;
		WingsInAction (false);
		yield return StartCoroutine (StopRolling ());
		_isRollingMaxSpeed = false;
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
			_coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed);
			yield return null;
		}
	}

	IEnumerator DashToTarget ()
	{
		var destPosition = _player.transform.position;
		var startPosition = host.transform.position;
		var distance = Vector3.Distance (_player.transform.position, host.transform.position);
		var velocity = dashingVelocity;
		var t = distance / velocity;
		var p = 0f;
		var currentAcceleration = host.agent.acceleration;
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
		host.agent.acceleration = currentAcceleration;
		// host.SetVelocity (Vector3.zero);
		// host.agent.velocity = Vector3.zero;
	}

	IEnumerator OnBeforeExecuting ()
	{
		host.StopMoving ();
		host.animator.enabled = false;
		yield break;
	}

	IEnumerator OnAfterExecuting ()
	{
		host.animator.enabled = true;
		host.animator.Play (defaultAnim.name, 0, 0);
		// host.KeepLeadingToTarget ();
		host.KeepMoving ();
		yield break;
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
		Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
	}
}
