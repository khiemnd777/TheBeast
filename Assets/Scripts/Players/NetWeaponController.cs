using Net;
using UnityEngine;

public class NetWeaponController : MonoBehaviour
{
  [SerializeField]
  NetIdentity _netIdentity;
  public NetMeleeHolderController meleeHolderController;
  public NetGunHolderController gunHolderController;
  public NetShieldHolderController shieldHolderController;
  string _typeOfWeapon;

  void Update()
  {
    if (_netIdentity.isLocal)
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (_typeOfWeapon != "gun")
        {
          _typeOfWeapon = "gun";
          DoActionOnGun();
          return;
        }
      }
      else if (Input.GetMouseButtonDown(1))
      {
        if (_typeOfWeapon != "melee")
        {
          _typeOfWeapon = "melee";
          DoActionOnMelee();
          return;
        }
      }
      else if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        if (_typeOfWeapon != "shield")
        {
          _typeOfWeapon = "shield";
          DoActionOnShield();
          return;
        }
      }
      if (_typeOfWeapon == "gun")
      {
        gunHolderController.DoUpdating();
      }
      if (_typeOfWeapon == "melee")
      {
        meleeHolderController.DoUpdating();
        shieldHolderController.DoUpdating();
      }
      if (_typeOfWeapon == "shield")
      {
        shieldHolderController.DoUpdating();
      }
    }
  }

  void DoActionOnGun()
  {
    shieldHolderController.TakeShieldDown();
    meleeHolderController.KeepMeleeInCover();
    gunHolderController.TakeGunUpArm();
    gunHolderController.DoUpdating();
  }

  void DoActionOnMelee()
  {
    shieldHolderController.TakeShieldDown();
    gunHolderController.KeepGunInCover();
    meleeHolderController.TakeMeleeUpArm();
    meleeHolderController.DoUpdating();
  }

  void DoActionOnShield()
  {
    gunHolderController.KeepGunInCover();
    meleeHolderController.KeepMeleeInCover();
    shieldHolderController.TakeShieldUpAsCover();
    shieldHolderController.DoUpdating();
  }
}
