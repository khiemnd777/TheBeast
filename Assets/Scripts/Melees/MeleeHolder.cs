using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHolder : MonoBehaviour
{
	public Melee melee;
	[SerializeField]
	Hand _hand;
	Melee _heldMelee;
	Vector3 _beginPosition;
	Animator _handAnimator;

	void Awake ()
	{
		_handAnimator = _hand.GetComponent<Animator> ();
	}

	void Start ()
	{
		_beginPosition = transform.localPosition;
		if (melee != null && melee is Object && !melee.Equals (null))
		{
			_heldMelee = Instantiate<Melee> (melee, transform.position, transform.rotation, transform);
			if (_handAnimator != null && _handAnimator is Object && !_handAnimator.Equals (null))
			{
				_handAnimator.runtimeAnimatorController = _heldMelee.meleeAnimatorController;
			}
			// _hand.maximumRange = _heldMelee.meleeHandType == MeleeHandType.OneHand ? 1 : .8f;
		}
	}

	public void BeforeHoldTrigger ()
	{
		StopCoroutine (TakeArmBackToBeginPosition ());
		transform.localPosition = _beginPosition;
	}

	public void HoldTrigger ()
	{
		if (_heldMelee != null && _heldMelee is Object && !_heldMelee.Equals (null))
		{
			// _heldMelee.HoldTrigger ();
		}
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
