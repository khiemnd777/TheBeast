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
	public float coreRotationScale;
	public float phase1Time;
	public float wingSpeed;
	[SerializeField]
	Transform _coreRotation;
	[SerializeField]
	AnimationCurve _phaseSpeedCurve;

	public override void Awake ()
	{
		base.Awake ();
		OnBeforeExecutingHandler += OnBeforeExecuting;
		OnAfterExecutingHandler += OnAfterExecuting;
	}

	public override IEnumerator OnExecuting ()
	{
		yield return StartCoroutine (Phase1 ());
		yield return StartCoroutine (Phase2 ());
	}

	IEnumerator Phase1 ()
	{
		headAnimator.Play (openFacesAnim.name, 0, 0);
		var speedRate = 0f;
		var coreScaleRate = 0f;
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / phase1Time;
			speedRate = Mathf.Lerp (0f, 1f, _phaseSpeedCurve.Evaluate (t));
			coreScaleRate = Mathf.Lerp (1f, coreRotationScale, _phaseSpeedCurve.Evaluate (t));
			_coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed * speedRate);
			_coreRotation.localScale = Vector3.one * coreScaleRate;
			yield return null;
		}
	}

	IEnumerator Phase2 ()
	{
		while (true)
		{
			_coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed);
			yield return null;
		}
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
		host.KeepMoving ();
		yield break;
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
		Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
	}
}
