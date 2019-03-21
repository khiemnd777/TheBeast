using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
	public Gun gun;
	Gun _heldGun;

	void Start ()
	{
		if (gun != null && gun is Object && !gun.Equals (null))
		{
			_heldGun = Instantiate<Gun> (gun, transform.position, transform.rotation, transform);
		}
	}

	public void HoldTrigger ()
	{
		if (_heldGun != null && _heldGun is Object && !_heldGun.Equals (null))
		{
			StartCoroutine (_heldGun.HoldTrigger ());
		}
	}
}
