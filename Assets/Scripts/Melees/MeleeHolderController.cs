using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHolderController : MonoBehaviour
{
	public MeleeHolder leftMeleeHolder;
	public MeleeHolder rightMeleeHolder;
	public float timeHoleLeftMeleeTrigger;

	DotSight _dotSight;
	bool _isLeft;
	bool _isMouseHoldingDown;
	float _timeForHoldLeftGunTrigger;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	public void DoUpdating ()
	{
		RotateMeleeHolder (leftMeleeHolder);
		RotateMeleeHolder (rightMeleeHolder);
		if (Input.GetMouseButtonDown (1))
		{
			_isMouseHoldingDown = true;
			// StartCoroutine (HoldTriggers ());
			HoldTriggers ();
		}
		if (Input.GetMouseButtonUp (1))
		{
			_isMouseHoldingDown = false;
			_timeForHoldLeftGunTrigger = 0f;
			ReleaseTriggers ();
		}
	}

	public void KeepMeleeInCover ()
	{
		KeepInCover (rightMeleeHolder);
		KeepInCover (leftMeleeHolder);
	}

	public void TakeMeleeUpArm ()
	{
		TakeUpArm (rightMeleeHolder);
		TakeUpArm (leftMeleeHolder);
	}

	// IEnumerator HoldTriggers ()
	// {
	// 	while (_isMouseHoldingDown)
	// 	{
	// 		HoldTrigger (rightMeleeHolder);
	// 		yield return new WaitForSeconds (.125f);
	// 		if (!_isMouseHoldingDown) yield break;
	// 		HoldTrigger (leftMeleeHolder);
	// 	}
	// }

	void HoldTriggers ()
	{
		if (!_isMouseHoldingDown) return;
		HoldTrigger (rightMeleeHolder);
		_timeForHoldLeftGunTrigger += Time.deltaTime / timeHoleLeftMeleeTrigger;
		if (_timeForHoldLeftGunTrigger >= 1f)
		{
			_timeForHoldLeftGunTrigger = 0f;
			HoldTrigger (leftMeleeHolder);
		}
	}

	void ReleaseTriggers ()
	{
		ReleaseTrigger (rightMeleeHolder);
		ReleaseTrigger (leftMeleeHolder);
	}

	void RotateMeleeHolder (MeleeHolder gunHolder)
	{
		if (gunHolder == null || gunHolder is Object && gunHolder.Equals (null)) return;
		var normal = _dotSight.NormalizeFromPoint (gunHolder.transform.position);
		var angle = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		gunHolder.transform.eulerAngles = new Vector3 (0, angle, 0);
	}

	void KeepInCover (MeleeHolder meleeHolder)
	{
		if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals (null))
		{
			meleeHolder.KeepInCover ();
		}
	}

	void TakeUpArm (MeleeHolder meleeHolder)
	{
		if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals (null))
		{
			meleeHolder.TakeUpArm ();
		}
	}

	void HoldTrigger (MeleeHolder meleeHolder)
	{
		if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals (null))
		{
			meleeHolder.HoldTrigger ();
		}
	}

	void ReleaseTrigger (MeleeHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			// gunHolder.ReleaseTrigger ();
		}
	}
}
