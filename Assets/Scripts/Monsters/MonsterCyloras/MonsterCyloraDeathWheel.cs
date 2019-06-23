using System;
using System.Collections;
using System.Collections.Generic;
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
	Player2 _player;
	bool _isStopRolling;

	public override void Awake ()
	{
		base.Awake ();
		_player = FindObjectOfType<Player2> ();
		OnBeforeExecutingHandler += OnBeforeExecuting;
		OnAfterExecutingHandler += OnAfterExecuting;
	}

	public override IEnumerator OnExecuting ()
	{
		yield return StartCoroutine (StartRolling ());
		StartCoroutine (KeepRolling ());
		yield return new WaitForSeconds (idleToDashTime);
		yield return StartCoroutine (DashToTarget ());
		yield return StartCoroutine (StopRolling ());
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
		_isStopRolling = false;
	}

	IEnumerator StopRolling ()
	{
		_isStopRolling = true;
		headAnimator.Play (closeFacesAnim.name, 0, 0);
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
		host.StopLeadingToTarget ();
		host.StopRotatingToTarget ();
		var destPosition = _player.transform.position;
		var startPosition = host.transform.position;
		var distance = Vector3.Distance (_player.transform.position, host.transform.position);
		var velocity = dashingVelocity;
		var t = distance / velocity;
		var p = 0f;
		while (p <= 1f)
		{
			p += Time.deltaTime / t;
			host.transform.position = Vector3.Lerp (startPosition, destPosition, p);
			yield return null;
		}
		host.KeepRotatingToTarget ();
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
		host.KeepLeadingToTarget ();
		host.KeepMoving ();
		yield break;
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
		Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
	}
}
