using Net;
using UnityEngine;

public class NetWeaponController : MonoBehaviour
{
  NetworkManager _networkManager;
  [SerializeField]
  NetIdentity _netIdentity;
  public NetMeleeHolderController meleeHolderController;
  public NetGunHolderController gunHolderController;
  public NetShieldHolderController shieldHolderController;
  string _typeOfWeapon;

  void Start()
  {
    _networkManager = NetworkManagerCache.networkManager;
    _netIdentity.onMessageReceived += (eventName, message) =>
    {
      switch (eventName)
      {
        case "gun_does_action":
          {
            DoActionOnGun();
          }
          break;
        case "melee_does_action":
          {
            DoActionOnMelee();
            meleeHolderController.HoldTriggers();
          }
          break;
        case "shield_does_action":
          {
            DoActionOnShield();
          }
          break;
        case "make_sure_gun_taken_up":
          {
            gunHolderController.TakeGunUpArm();
          }
          break;
        case "make_sure_melee_taken_up":
          {
            meleeHolderController.TakeMeleeUpArm();
          }
          break;
        default:
          break;
      }
    };
  }

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
          gunHolderController.DoUpdating();
          EmitDoActionOnGun();
          return;
        }
      }
      else if (Input.GetMouseButtonDown(1))
      {
        if (gunHolderController.secondAction)
        {
          return;
        }
        if (_typeOfWeapon != "melee")
        {
          _typeOfWeapon = "melee";
          DoActionOnMelee();
          meleeHolderController.DoUpdating();
          EmitDoActionOnMelee();
          return;
        }
      }
      else if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        if (_typeOfWeapon != "shield")
        {
          _typeOfWeapon = "shield";
          DoActionOnShield();
          shieldHolderController.DoUpdating();
          EmitDoActionOnShield();
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
  }

  void EmitDoActionOnGun()
  {
    _netIdentity.EmitMessage("gun_does_action", null);
  }

  void DoActionOnMelee()
  {
    shieldHolderController.TakeShieldDown();
    gunHolderController.KeepGunInCover();
    meleeHolderController.TakeMeleeUpArm();
  }

  void EmitDoActionOnMelee()
  {
    _netIdentity.EmitMessage("melee_does_action", null);
  }

  void DoActionOnShield()
  {
    gunHolderController.KeepGunInCover();
    meleeHolderController.KeepMeleeInCover();
    shieldHolderController.TakeShieldUpAsCover();
  }

  void EmitDoActionOnShield()
  {
    _netIdentity.EmitMessage("shield_does_action", null);
  }
}
