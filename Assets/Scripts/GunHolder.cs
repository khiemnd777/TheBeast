﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
	public Gun gun;
	[SerializeField]
	Hand _hand;
	Gun _heldGun;
	Vector3 _beginPosition;

	void Start ()
	{
		_beginPosition = transform.localPosition;
		if (gun != null && gun is Object && !gun.Equals (null))
		{
			_heldGun = Instantiate<Gun> (gun, transform.position, transform.rotation, transform);
			_heldGun.OnProjectileLaunched += OnProjectileLaunched;
			_hand.maximumRange = _heldGun.gunHandType == GunHandType.OneHand ? 1 : .8f;
		}
	}

	public void BeforeHoldTrigger ()
	{
		StopCoroutine (TakeArmBackToBeginPosition ());
		transform.localPosition = _beginPosition;
	}

	public void HoldTrigger ()
	{
		if (_heldGun != null && _heldGun is Object && !_heldGun.Equals (null))
		{
			StartCoroutine (_heldGun.HoldTrigger ());
		}
	}

	void OnProjectileLaunched ()
	{
		// knock arm back
		StartCoroutine (KnockArmBack ());
	}

	IEnumerator KnockArmBack ()
	{
		var expectedKnockbackX = _beginPosition.x - _heldGun.knockbackIndex;
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / .125f;
			var knockbackX = Mathf.Lerp (_beginPosition.x, expectedKnockbackX, t);
			var knockbackPosition = new Vector3 (knockbackX, _beginPosition.y, _beginPosition.z);
			transform.localPosition = knockbackPosition;
			yield return null;
		}
		StartCoroutine (TakeArmBackToBeginPosition ());
	}

	IEnumerator TakeArmBackToBeginPosition ()
	{
		var currentPosition = transform.localPosition;
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / .08f;
			transform.localPosition = Vector3.Lerp (currentPosition, _beginPosition, t);
			yield return null;
		}
	}
}
