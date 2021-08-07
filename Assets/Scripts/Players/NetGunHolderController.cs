using Net;
using UnityEngine;

public class NetGunHolderController : MonoBehaviour
{
  [SerializeField]
  NetIdentity _netIdentity;

  public NetGunHolder leftGunHolder;
  public NetGunHolder rightGunHolder;
  public float timeHoleLeftGunTrigger;

  DotSightController _dotSightController;
  DotSight _dotSight;
  bool _isLeft;
  bool _isMouseHoldingDown;
  float _timeForHoldLeftGunTrigger;

  void Start()
  {
    if (_netIdentity.isLocal)
    {
      _dotSightController = FindObjectOfType<DotSightController>();
      if (_dotSightController)
      {
        _dotSight = _dotSightController.dotSight;
      }
    }
    _netIdentity.onMessageReceived += (eventName, message) =>
    {
      switch (eventName)
      {
        case "left_gun_rotate_towards":
          {
            if (_netIdentity.isClient && !_netIdentity.isLocal)
            {
              var rotationJson = Utility.Deserialize<GunRotateTowardsJson>(message);
              var gunRotation = Utility.AnglesArrayToQuaternion(rotationJson.rotation);
              leftGunHolder.RotateTowards(gunRotation);
            }
          }
          break;
        case "right_gun_rotate_towards":
          {
            if (_netIdentity.isClient && !_netIdentity.isLocal)
            {
              var rotationJson = Utility.Deserialize<GunRotateTowardsJson>(message);
              var gunRotation = Utility.AnglesArrayToQuaternion(rotationJson.rotation);
              leftGunHolder.RotateTowards(gunRotation);
            }
          }
          break;
        default:
          break;
      }
    };
  }

  public void DoUpdating()
  {
    RotateGunHolder(leftGunHolder);
    RotateGunHolder(rightGunHolder);
    EmitGunHolderRotateTowards(leftGunHolder, true);
    EmitGunHolderRotateTowards(rightGunHolder, false);
    if (Input.GetMouseButtonDown(0))
    {
      _isMouseHoldingDown = true;
    }
    if (Input.GetMouseButtonUp(0))
    {
      _isMouseHoldingDown = false;
      _timeForHoldLeftGunTrigger = 0f;
      ReleaseTriggers();
    }
    HoldTriggers();
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
    _timeForHoldLeftGunTrigger += Time.deltaTime / timeHoleLeftGunTrigger;
    if (_timeForHoldLeftGunTrigger >= 1f)
    {
      _timeForHoldLeftGunTrigger = 0f;
      HoldTrigger(leftGunHolder);
    }
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

  public void EmitGunHolderRotateTowards(NetGunHolder gunHolder, bool isLeftSide)
  {
    var eventName = isLeftSide ? "left_gun_rotate_towards" : "right_gun_rotate_towards";
    _netIdentity.EmitMessage(eventName, new GunRotateTowardsJson
    {
      rotation = Utility.QuaternionToAnglesArray(gunHolder.transform.rotation)
    });
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
      gunHolder.BeforeHoldTrigger();
      gunHolder.HoldTrigger();
    }
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
