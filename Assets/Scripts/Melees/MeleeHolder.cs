using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHolder : MonoBehaviour
{
	public Melee melee;
	public float speed;
	[SerializeField]
	Hand _hand;
	[SerializeField]
	Player2 _player;
	Melee _heldMelee;
	Vector3 _beginPosition;
	Animator _handAnimator;
	bool _isHoldingOn;

	void Awake ()
	{
		_handAnimator = _hand.GetComponent<Animator> ();
	}

	public void KeepInCover ()
	{
		if (_heldMelee != null && _heldMelee is Object && !_heldMelee.Equals (null))
		{
			_heldMelee.KeepInCover ();
		}
	}

	public void TakeUpArm ()
	{
		if (melee != null && melee is Object && !melee.Equals (null))
		{
			_heldMelee = Instantiate<Melee> (melee, transform.position, transform.rotation, transform);
			_heldMelee.TakeUpArm (this, _hand, _handAnimator, _player);
			_hand.maximumRange = 1.4f;
			if (_handAnimator != null && _handAnimator is Object && !_handAnimator.Equals (null))
			{
				_handAnimator.runtimeAnimatorController = _heldMelee.meleeAnimatorController;
			}
		}
	}

	public void HoldTrigger ()
	{
		if (_heldMelee != null && _heldMelee is Object && !_heldMelee.Equals (null))
		{
			if (_isHoldingOn) return;
			StartCoroutine (OnHoldingTrigger ());
		}
	}

	IEnumerator OnHoldingTrigger ()
	{
		_isHoldingOn = true;
		yield return StartCoroutine (_heldMelee.HoldTrigger ());
		yield return StartCoroutine (WaitingForNextMeleeBeOnTrigger ());
		_isHoldingOn = false;
	}

	IEnumerator WaitingForNextMeleeBeOnTrigger ()
	{
		var t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 10f * speed;
			yield return null;
		}
	}
}
