using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHolder : MonoBehaviour
{
	public Melee melee;
	[System.NonSerialized]
	public Melee heldMelee;
	public float delay;
	[SerializeField]
	Hand _hand;
	[SerializeField]
	Player2 _player;
	Vector3 _beginPosition;
	Animator _handAnimator;
	bool _isHoldingOn;

	void Awake ()
	{
		_handAnimator = _hand.GetComponent<Animator> ();
	}

	public void KeepInCover ()
	{
		if (heldMelee != null && heldMelee is Object && !heldMelee.Equals (null))
		{
			heldMelee.KeepInCover ();
		}
	}

	public void TakeUpArm ()
	{
		if (melee != null && melee is Object && !melee.Equals (null))
		{
			if (!heldMelee)
			{
				heldMelee = Instantiate<Melee> (melee, transform.position, transform.rotation, transform);
			}
			heldMelee.TakeUpArm (this, _hand, _handAnimator, _player);
			_hand.maximumRange = 1.4f;
			if (_handAnimator != null && _handAnimator is Object && !_handAnimator.Equals (null))
			{
				_handAnimator.runtimeAnimatorController = heldMelee.meleeAnimatorController;
			}
		}
	}

	public void HoldTrigger ()
	{
		if (heldMelee != null && heldMelee is Object && !heldMelee.Equals (null))
		{
			if (_isHoldingOn) return;
			StartCoroutine (OnHoldingTrigger ());
		}
	}

	IEnumerator OnHoldingTrigger ()
	{
		_isHoldingOn = true;
		yield return StartCoroutine (heldMelee.HoldTrigger ());
		// yield return StartCoroutine (WaitingForNextMeleeBeOnTrigger ());
		_isHoldingOn = false;
	}

	IEnumerator WaitingForNextMeleeBeOnTrigger ()
	{
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / delay;
			yield return null;
		}
	}
}
