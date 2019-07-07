using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHolderController : MonoBehaviour
{
	public MeleeHolder rightMeleeHolder;
	DotSight _dotSight;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	public void DoUpdating ()
	{
		RotateMeleeHolder (rightMeleeHolder);
		if (Input.GetMouseButtonDown (0))
		{
			HoldTriggers ();
		}
	}

	public void KeepMeleeInCover ()
	{
		KeepInCover (rightMeleeHolder);
	}

	public void TakeMeleeUpArm ()
	{
		TakeUpArm (rightMeleeHolder);
	}

	void HoldTriggers ()
	{
		HoldTrigger (rightMeleeHolder);
	}

	void ReleaseTriggers ()
	{
		ReleaseTrigger (rightMeleeHolder);
	}

	void RotateMeleeHolder (MeleeHolder meleeHolder)
	{
		if (meleeHolder == null || meleeHolder is Object && meleeHolder.Equals (null)) return;
		var normal = _dotSight.NormalizeFromPoint (meleeHolder.transform.position);
		var angle = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		var destRot = Utilities.RotateByNormal (normal, Vector3.up);
		var meleeHolderTransform = meleeHolder.transform;
		meleeHolderTransform.rotation = Quaternion.RotateTowards (meleeHolderTransform.rotation, destRot, Time.deltaTime * 630f);
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

	void ReleaseTrigger (MeleeHolder meleeHolder)
	{
		if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals (null))
		{
			// gunHolder.ReleaseTrigger ();
		}
	}
}
