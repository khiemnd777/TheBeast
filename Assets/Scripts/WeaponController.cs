﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	[SerializeField]
	MeleeHolderController _meleeHolderController;
	[SerializeField]
	GunHolderController _gunHolderController;
	[SerializeField]
	ShieldHolderController _shieldHolderController;
	string typeOfWeapon;

	void Update ()
	{
		if (Input.GetMouseButtonDown (1))
		{
			if (typeOfWeapon != "gun")
			{
				typeOfWeapon = "gun";
				DoActionOnGun ();
				return;
			}
		}
		else if (Input.GetMouseButtonDown (0))
		{
			if (typeOfWeapon != "melee")
			{
				typeOfWeapon = "melee";
				DoActionOnMelee ();
				return;
			}
		}
		else if (Input.GetKeyDown (KeyCode.LeftShift))
		{
			if (typeOfWeapon != "shield")
			{
				typeOfWeapon = "shield";
				DoActionOnShield ();
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
		if (typeOfWeapon == "shield")
		{
			_shieldHolderController.DoUpdating ();
		}
	}

	void DoActionOnGun ()
	{
		_shieldHolderController.TakeShieldDown ();
		_meleeHolderController.KeepMeleeInCover ();
		_gunHolderController.TakeGunUpArm ();
		// _gunHolderController.DoUpdating ();
	}

	void DoActionOnMelee ()
	{
		_shieldHolderController.TakeShieldDown ();
		_gunHolderController.KeepGunInCover ();
		_meleeHolderController.TakeMeleeUpArm ();
		_meleeHolderController.DoUpdating ();
	}

	void DoActionOnShield ()
	{
		_gunHolderController.KeepGunInCover ();
		_meleeHolderController.KeepMeleeInCover ();
		_shieldHolderController.TakeShieldUpAsCover ();
		_shieldHolderController.DoUpdating ();
	}
}
