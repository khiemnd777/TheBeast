﻿using Net;
using UnityEngine;

public class NetGunHolderController : MonoBehaviour
{
  [SerializeField]
  NetIdentity _netIdentity;

  public NetGunHolder leftGunHolder;
  public NetGunHolder rightGunHolder;
  public float timeHoleLeftGunTrigger;
  Cooldown _holeLeftGunTriggerCooldown;

  [SerializeField]
  float _emitGunHolderRotateTowardsInterval = .2f;
  Cooldown _emitLeftGunHolderRotateTowardsCooldown;
  Cooldown _emitRightGunHolderRotateTowardsCooldown;

  DotSightController _dotSightController;
  DotSight _dotSight;
  bool _isLeft;
  bool _isMouseHoldingDown;
  float _timeForHoldLeftGunTrigger;

  public bool secondAction
  {
    get => leftGunHolder.secondAction || rightGunHolder.secondAction;
  }

  void Start()
  {
    if (_netIdentity.isLocal)
    {
      _dotSightController = FindObjectOfType<DotSightController>();
      if (_dotSightController)
      {
        _dotSight = _dotSightController.dotSight;
      }
      _holeLeftGunTriggerCooldown = new Cooldown(HoldTriggerLeftGun);
      _emitLeftGunHolderRotateTowardsCooldown = new Cooldown(EmitGunHolderRotateTowards(leftGunHolder, true));
      _emitRightGunHolderRotateTowardsCooldown = new Cooldown(EmitGunHolderRotateTowards(rightGunHolder, false));
    }
    if (_netIdentity.isClient)
    {
      _netIdentity.onMessageReceived += OnReceivedGunHolderRotateTowards;
    }
  }

  public void DoUpdating()
  {
    if (_netIdentity.isLocal)
    {
      RotateGunHolder(leftGunHolder);
      RotateGunHolder(rightGunHolder);
      _emitLeftGunHolderRotateTowardsCooldown.Count(_emitGunHolderRotateTowardsInterval);
      _emitLeftGunHolderRotateTowardsCooldown.Execute();
      _emitRightGunHolderRotateTowardsCooldown.Count(_emitGunHolderRotateTowardsInterval);
      _emitRightGunHolderRotateTowardsCooldown.Execute();
      if (Input.GetMouseButtonDown(0))
      {
        _isMouseHoldingDown = true;
        _netIdentity.EmitMessage("make_sure_gun_taken_up", null);
      }
      else if (Input.GetMouseButtonUp(0))
      {
        _isMouseHoldingDown = false;
        _timeForHoldLeftGunTrigger = 0f;
        ReleaseTriggers();
      }
      else if (Input.GetMouseButtonDown(1))
      {
        if (this.secondAction)
        {
          this.OnSecondAction();
        }
      }
      HoldTriggers();
    }
  }

  public void OnSecondAction()
  {
    if (leftGunHolder.secondAction)
    {
      leftGunHolder.OnSecondAction(_dotSight.GetCurrentPoint());
    }
    if (rightGunHolder.secondAction)
    {
      rightGunHolder.OnSecondAction(_dotSight.GetCurrentPoint());
    }
  }

  public void KeepGunInCover()
  {
    KeepInCover(rightGunHolder);
    KeepInCover(leftGunHolder);
  }

  public void TakeGunUpArm()
  {
    TakeUpArm(rightGunHolder);
    TakeUpArm(leftGunHolder);
  }

  void HoldTriggers()
  {
    if (!_isMouseHoldingDown) return;

    HoldTrigger(rightGunHolder);
    _holeLeftGunTriggerCooldown.Count(timeHoleLeftGunTrigger);
    _holeLeftGunTriggerCooldown.Execute();
  }

  void ReleaseTriggers()
  {
    ReleaseTrigger(rightGunHolder);
    ReleaseTrigger(leftGunHolder);
  }

  void RotateGunHolder(NetGunHolder gunHolder)
  {
    if (gunHolder == null || gunHolder is Object && gunHolder.Equals(null)) return;
    if (!_dotSight) return;
    var normal = _dotSight.NormalizeFromPoint(gunHolder.transform.position);
    var destRot = Utility.RotateByNormal(normal, Vector3.up);
    gunHolder.RotateTowards(destRot);
  }

  public System.Action EmitGunHolderRotateTowards(NetGunHolder gunHolder, bool isLeftSide)
  {
    return new System.Action(() =>
    {
      var eventName = isLeftSide ? "left_gun_rotate_towards" : "right_gun_rotate_towards";
      _netIdentity.EmitMessage(eventName, new GunRotateTowardsJson
      {
        rotation = Utility.QuaternionToAnglesArray(gunHolder.transform.rotation)
      });
    });
  }

  void OnReceivedGunHolderRotateTowards(string eventName, string message)
  {
    switch (eventName)
    {
      case "left_gun_rotate_towards":
        {
          var rotationJson = Utility.Deserialize<GunRotateTowardsJson>(message);
          var gunRotation = Utility.AnglesArrayToQuaternion(rotationJson.rotation);
          leftGunHolder.RotateTowards(gunRotation);
        }
        break;
      case "right_gun_rotate_towards":
        {
          var rotationJson = Utility.Deserialize<GunRotateTowardsJson>(message);
          var gunRotation = Utility.AnglesArrayToQuaternion(rotationJson.rotation);
          rightGunHolder.RotateTowards(gunRotation);
        }
        break;
      default:
        break;
    }
  }

  void KeepInCover(NetGunHolder gunHolder)
  {
    if (gunHolder != null && gunHolder is Object && !gunHolder.Equals(null))
    {
      gunHolder.KeepInCover();
    }
  }

  void TakeUpArm(NetGunHolder gunHolder)
  {
    if (gunHolder != null && gunHolder is Object && !gunHolder.Equals(null))
    {
      gunHolder.TakeUpArm();
    }
  }

  void HoldTrigger(NetGunHolder gunHolder)
  {
    if (gunHolder != null && gunHolder is Object && !gunHolder.Equals(null))
    {
      // Get dot sight position as destination.
      gunHolder.BeforeHoldTrigger();
      gunHolder.HoldTrigger(_dotSight.GetCurrentPoint());
    }
  }

  void HoldTriggerLeftGun()
  {
    HoldTrigger(leftGunHolder);
  }

  void ReleaseTrigger(NetGunHolder gunHolder)
  {
    if (gunHolder != null && gunHolder is Object && !gunHolder.Equals(null))
    {
      gunHolder.ReleaseTrigger();
    }
  }
}

public struct GunRotateTowardsJson
{
  public float[] rotation;
}
