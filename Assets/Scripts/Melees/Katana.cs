using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	public float hitback;
	Player2 _player;
	Hand _hand;
	Animator _handAnimator;
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
		_player.RegisterLock ("Katana");
	}

	public override void HoldTrigger ()
	{
		if (_inAnyAction) return;
		_inAnyAction = true;
		_hand.enabled = false;
		_handAnimator.enabled = true;
		StartCoroutine (EndOfAnimation ());
		_player.Lock ("Katana");
		var slashAnimName = _slashCount++ % 2 == 0 ? "Katana Slash" : "Katana Slash 2";
		_handAnimator.Play (slashAnimName, 0, 0);
	}

	public override void TakeUpArm (Hand hand, Animator handAnimator, Player2 player)
	{
		_hand = hand;
		_handAnimator = handAnimator;
		_player = player;
	}

	public override void KeepInCover ()
	{
		_handAnimator.enabled = false;
		_hand.enabled = true;
		_player.Unlock ("Katana");
		_inAnyAction = false;
		_trail.enabled = false;
		base.KeepInCover ();
	}

	IEnumerator EndOfAnimation ()
	{
		_trail.enabled = true;
		var currentAnimatorStateInfo = _handAnimator.GetCurrentAnimatorStateInfo (0);
		yield return new WaitForSeconds (currentAnimatorStateInfo.length);
		_handAnimator.enabled = false;
		_hand.enabled = true;
		_player.Unlock ("Katana");
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
				var dir = _player.transform.position - contactPoint;
				dir.Normalize ();
				hitMonster.OnHit (transform, hitback, dir, contactPoint);
				_slowMotionMonitor.Freeze (.45f, .2f);
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
