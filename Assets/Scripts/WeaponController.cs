using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	[SerializeField]
	MeleeHolderController _meleeHolderController;
	[SerializeField]
	GunHolderController _gunHolderController;
	string typeOfWeapon;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			if (typeOfWeapon != "gun")
			{
				typeOfWeapon = "gun";
				_meleeHolderController.KeepMeleeInCover ();
				_gunHolderController.TakeGunUpArm ();
				_gunHolderController.DoUpdating ();
				return;
			}
		}
		else if (Input.GetMouseButtonDown (1))
		{
			if (typeOfWeapon != "melee")
			{
				typeOfWeapon = "melee";
				_gunHolderController.KeepGunInCover ();
				_meleeHolderController.TakeMeleeUpArm ();
				_meleeHolderController.DoUpdating ();
				return;
			}
		}
		if (typeOfWeapon == "gun")
		{
			_gunHolderController.DoUpdating ();
		}
		if (typeOfWeapon == "melee")
		{
			_meleeHolderController.DoUpdating ();
		}
	}
}
