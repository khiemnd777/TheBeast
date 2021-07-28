using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	public MeleeHolderController meleeHolderController;
	public GunHolderController gunHolderController;
	public ShieldHolderController shieldHolderController;
	string typeOfWeapon;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			if (typeOfWeapon != "gun")
			{
				typeOfWeapon = "gun";
				DoActionOnGun ();
				return;
			}
		}
		else if (Input.GetMouseButtonDown (1))
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
			gunHolderController.DoUpdating ();
		}
		if (typeOfWeapon == "melee")
		{
			meleeHolderController.DoUpdating ();
			shieldHolderController.DoUpdating ();
		}
		if (typeOfWeapon == "shield")
		{
			shieldHolderController.DoUpdating ();
		}
	}

	void DoActionOnGun ()
	{
		shieldHolderController.TakeShieldDown ();
		meleeHolderController.KeepMeleeInCover ();
		gunHolderController.TakeGunUpArm ();
		gunHolderController.DoUpdating ();
	}

	void DoActionOnMelee ()
	{
		shieldHolderController.TakeShieldDown ();
		gunHolderController.KeepGunInCover ();
		meleeHolderController.TakeMeleeUpArm ();
		meleeHolderController.DoUpdating ();
	}

	void DoActionOnShield ()
	{
		gunHolderController.KeepGunInCover ();
		// _meleeHolderController.KeepMeleeInCover ();
		shieldHolderController.TakeShieldUpAsCover ();
		shieldHolderController.DoUpdating ();
	}
}
