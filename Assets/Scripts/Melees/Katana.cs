using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	public float hitback;
	bool _inAnyAction;
	int _slashCount;
	BoxCollider _collider;
	[SerializeField]
	TrailRenderer _trail;
	SlowMotionMonitor _slowMotionMonitor;

	public override void Awake ()
	{
		base.Awake ();
		_collider = GetComponent<BoxCollider> ();
		_slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
	}

	public override void Start ()
	{
		base.Start ();
		player.RegisterLock ("Katana");
	}

	public override void HoldTrigger (Hand hand, Animator handAnimator)
	{
		if (_inAnyAction) return;
		_inAnyAction = true;
		hand.enabled = false;
		handAnimator.enabled = true;
		StartCoroutine (EndOfAnimation (hand, handAnimator));
		player.Lock ("Katana");
		var slashAnimName = _slashCount++ % 2 == 0 ? "Katana Slash" : "Katana Slash 2";
		handAnimator.Play (slashAnimName, 0, 0);
	}

	IEnumerator EndOfAnimation (Hand hand, Animator handAnimator)
	{
		_trail.enabled = true;
		var currentAnimatorStateInfo = handAnimator.GetCurrentAnimatorStateInfo (0);
		yield return new WaitForSeconds (currentAnimatorStateInfo.length);
		handAnimator.enabled = false;
		hand.enabled = true;
		player.Unlock ("Katana");
		_inAnyAction = false;
		_trail.enabled = false;
	}

	void OnTriggerEnter (Collider other)
	{
		if (!_inAnyAction) return;
		if (other)
		{
			var hitMonster = other.GetComponent<Monster> ();
			if (hitMonster)
			{
				var contactPoint = other.ClosestPointOnBounds (transform.position);
				var dir = contactPoint - player.transform.position;
				dir.Normalize ();
				hitMonster.OnHit (transform, hitback, dir, contactPoint);
				return;
			}
			var reversedDamage = other.GetComponent<ReversedDamage> ();
			if (reversedDamage)
			{
				reversedDamage.reversed = true;
				reversedDamage.speed *= 1.25f;
				_slowMotionMonitor.Freeze (.009f, 1.5f);
				return;
			}
		}
	}
}
