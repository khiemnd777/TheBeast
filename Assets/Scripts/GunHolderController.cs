using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderController : MonoBehaviour
{
	public GunHolder leftGunHolder;
	public GunHolder rightGunHolder;

	DotSight _dotSight;
	bool _isLeft;
	bool _isMouseHoldingDown;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	void Update ()
	{
		RotateGunHolder (leftGunHolder);
		RotateGunHolder (rightGunHolder);
		if (Input.GetMouseButtonDown (0))
		{
			_isMouseHoldingDown = true;
			StartCoroutine (HoldTriggers ());
		}
		if (Input.GetMouseButtonUp (0))
		{
			_isMouseHoldingDown = false;
			ReleaseTriggers ();
		}
	}

	IEnumerator HoldTriggers ()
	{
		while (_isMouseHoldingDown)
		{
			HoldTrigger (rightGunHolder);
			yield return new WaitForSeconds (.125f);
			if (!_isMouseHoldingDown) yield break;
			HoldTrigger (leftGunHolder);
			yield return null;
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
		var normal = _dotSight.NormalizeFromPoint (gunHolder.transform.position);
		var angle = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		gunHolder.transform.eulerAngles = new Vector3 (0, angle, 0);
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
