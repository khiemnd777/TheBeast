using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	public float hitback;
	bool _inAnAction;
	int _slashCount;
	BoxCollider _collider;
	[SerializeField]
	TrailRenderer _trail;

	public override void Awake ()
	{
		base.Awake ();
		_collider = GetComponent<BoxCollider> ();
	}

	public override void Start ()
	{
		base.Start ();
		player.RegisterLock ("Katana");
	}

	public override void HoldTrigger (Hand hand, Animator handAnimator)
	{
		if (_inAnAction) return;
		_inAnAction = true;
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
		// var t = 0f;
		// while (t <= 1f)
		// {
		// 	t += Time.deltaTime / currentAnimatorStateInfo.length;
		// 	yield return null;
		// }
		yield return new WaitForSeconds (currentAnimatorStateInfo.length);
		handAnimator.enabled = false;
		hand.enabled = true;
		player.Unlock ("Katana");
		_inAnAction = false;
		_trail.enabled = false;
	}

	void OnTriggerEnter (Collider other)
	{
		if (!_inAnAction) return;
		if (other)
		{
			var hitMonster = other.GetComponent<Monster> ();
			if (hitMonster)
			{
				var contactPoint = other.ClosestPointOnBounds (transform.position);
				var dir = contactPoint - player.transform.position;
				dir.Normalize ();
				hitMonster.OnHit (transform, hitback, -dir, contactPoint);
				return;
			}
			var reversedDamage = other.GetComponent<ReversedDamage> ();
			if (reversedDamage)
			{
				reversedDamage.reversed = true;
				reversedDamage.speed *= 1.25f;
			}
		}
	}
}
