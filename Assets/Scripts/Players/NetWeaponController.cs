using Net;
using UnityEngine;

public class NetWeaponController : MonoBehaviour
{
  NetworkManager _networkManager;

  [SerializeField]
  NetIdentity _netIdentity;

  [SerializeField]
  Player _player;

  public NetMeleeHolderController meleeHolderController;
  public NetGunHolderController gunHolderController;
  public NetShieldHolderController shieldHolderController;


  string _typeOfWeapon;
  bool _surfing;

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
        if (_surfing) return;
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
        if (_surfing) return;
        if (!gunHolderController.secondAction)
        {
          if (_typeOfWeapon != "melee")
          {
            _typeOfWeapon = "melee";
            DoActionOnMelee();
            meleeHolderController.DoUpdating();
            EmitDoActionOnMelee();
            return;
          }
        }
      }
      else if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        // if (_typeOfWeapon != "shield")
        // {
        //   _typeOfWeapon = "shield";
        //   DoActionOnShield();
        //   shieldHolderController.DoUpdating();
        //   EmitDoActionOnShield();
        //   return;
        // }
        _player.gunWeightIncrement = 1f;
        _surfing = true;
        _typeOfWeapon = string.Empty;
        gunHolderController.KeepGunInCover();
        meleeHolderController.KeepMeleeInCover();
        shieldHolderController.TakeShieldDown();
      }
      else if (Input.GetKeyUp(KeyCode.LeftShift))
      {
        _surfing = false;
      }
      else if (Input.GetKeyDown(KeyCode.Q))
      {
        _player.gunWeightIncrement = 1f;
        _typeOfWeapon = string.Empty;
        gunHolderController.KeepGunInCover();
        meleeHolderController.KeepMeleeInCover();
        shieldHolderController.TakeShieldDown();
        return;
      }
      if (_typeOfWeapon == "gun")
      {
        if (_surfing) return;
        gunHolderController.DoUpdating();
      }
      if (_typeOfWeapon == "melee")
      {
        if (_surfing) return;
        meleeHolderController.DoUpdating();
        shieldHolderController.DoUpdating();
      }
      if (_typeOfWeapon == "shield")
      {
        if (_surfing) return;
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
