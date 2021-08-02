using UnityEngine;

public class GunHolderController : MonoBehaviour
{
	public GunHolder leftGunHolder;
	public GunHolder rightGunHolder;
	public float timeHoleLeftGunTrigger;

	DotSightController _dotSightController;
	bool _isLeft;
	bool _isMouseHoldingDown;
	float _timeForHoldLeftGunTrigger;

	void Awake ()
	{
		_dotSightController = FindObjectOfType<DotSightController> ();
	}

	public void DoUpdating ()
	{
		RotateGunHolder (leftGunHolder);
		RotateGunHolder (rightGunHolder);
		if (Input.GetMouseButtonDown (0))
		{
			_isMouseHoldingDown = true;
			// StartCoroutine (HoldTriggers ());
			// HoldTriggers ();
		}
		if (Input.GetMouseButtonUp (0))
		{
			_isMouseHoldingDown = false;
			_timeForHoldLeftGunTrigger = 0f;
			ReleaseTriggers ();
		}
		HoldTriggers ();
	}

	// IEnumerator HoldTriggers ()
	// {
	// 	while (_isMouseHoldingDown)
	// 	{
	// 		HoldTrigger (rightGunHolder);
	// 		yield return new WaitForSeconds (.125f);
	// 		if (!_isMouseHoldingDown) yield break;
	// 		HoldTrigger (leftGunHolder);
	// 	}
	// }

	public void KeepGunInCover ()
	{
		KeepInCover (rightGunHolder);
		KeepInCover (leftGunHolder);
	}

	public void TakeGunUpArm ()
	{
		TakeUpArm (rightGunHolder);
		TakeUpArm (leftGunHolder);
	}

	void HoldTriggers ()
	{
		if (!_isMouseHoldingDown) return;
		
		HoldTrigger (rightGunHolder);
		_timeForHoldLeftGunTrigger += Time.deltaTime / timeHoleLeftGunTrigger;
		if (_timeForHoldLeftGunTrigger >= 1f)
		{
			_timeForHoldLeftGunTrigger = 0f;
			HoldTrigger (leftGunHolder);
		}
	}

	void ReleaseTriggers ()
	{
		ReleaseTrigger (rightGunHolder);
		ReleaseTrigger (leftGunHolder);
	}

	void RotateGunHolder (GunHolder gunHolder)
	{
		if (gunHolder == null || gunHolder is Object && gunHolder.Equals (null)) return;
		var normal = _dotSightController.NormalizeFromPoint (gunHolder.transform.position);
		var destRot = Utility.RotateByNormal (normal, Vector3.up);
		var gunHolderTransform = gunHolder.transform;
		gunHolderTransform.rotation = Quaternion.RotateTowards(gunHolderTransform.rotation, destRot, Time.deltaTime * 630f);
	}

	void KeepInCover (GunHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			gunHolder.KeepInCover ();
		}
	}

	void TakeUpArm (GunHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			gunHolder.TakeUpArm ();
		}
	}

	void HoldTrigger (GunHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			gunHolder.BeforeHoldTrigger ();
			gunHolder.HoldTrigger ();
		}
	}

	void ReleaseTrigger (GunHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			gunHolder.ReleaseTrigger ();
		}
	}
}
