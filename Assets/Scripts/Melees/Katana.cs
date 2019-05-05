using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	bool _inAnAction;
	int _slashCount;

	public override void Start ()
	{
		name = "Katana";
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
		var currentAnimatorStateInfo = handAnimator.GetCurrentAnimatorStateInfo (0);
		// var t = 0f;
		// while (t <= 1f)
		// {
		// 	t += Time.deltaTime / currentAnimatorClip.Length;
		// 	yield return null;
		// }
		yield return new WaitForSeconds (currentAnimatorStateInfo.length);
		handAnimator.enabled = false;
		hand.enabled = true;
		player.Unlock ("Katana");
		_inAnAction = false;
	}
}
