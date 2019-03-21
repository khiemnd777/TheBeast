using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderController : MonoBehaviour
{
	public GunHolder leftGunHolder;
	public GunHolder rightGunHolder;

	DotSight _dotSight;
	bool _isLeft;

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
			StartCoroutine (HoldTrigger ());
		}
	}

	IEnumerator HoldTrigger ()
	{
		HoldTrigger (leftGunHolder);
		yield return new WaitForSeconds (.125f);
		HoldTrigger (rightGunHolder);
	}

	void RotateGunHolder (GunHolder gunHolder)
	{
		if (gunHolder == null || gunHolder is Object && gunHolder.Equals (null)) return;
		var normal = _dotSight.NormalizeFromPoint (gunHolder.transform.position);
		var angle = Mathf.Atan2 (normal.y, normal.x) * Mathf.Rad2Deg;
		gunHolder.transform.eulerAngles = new Vector3 (0, 0, angle);
	}

	void HoldTrigger (GunHolder gunHolder)
	{
		if (gunHolder != null && gunHolder is Object && !gunHolder.Equals (null))
		{
			gunHolder.HoldTrigger ();
		}
	}
}
