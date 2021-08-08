using Net;
using UnityEngine;

public class NetMeleeHolderController : MonoBehaviour
{
  [SerializeField]
  NetIdentity _netIdentity;
  public NetMeleeHolder rightMeleeHolder;
  DotSightController _dotSightController;
  DotSight _dotSight;

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
  }

  public void DoUpdating()
  {
    if (_netIdentity.isLocal)
    {
      RotateMeleeHolder(rightMeleeHolder);
      if (Input.GetMouseButtonDown(1))
      {
        HoldTriggers();
      }
    }
  }

  public void KeepMeleeInCover()
  {
    KeepInCover(rightMeleeHolder);
  }

  public void TakeMeleeUpArm()
  {
    TakeUpArm(rightMeleeHolder);
  }

  void HoldTriggers()
  {
    HoldTrigger(rightMeleeHolder);
  }

  void ReleaseTriggers()
  {
    ReleaseTrigger(rightMeleeHolder);
  }

  void RotateMeleeHolder(NetMeleeHolder meleeHolder)
  {
    if (meleeHolder == null || meleeHolder is Object && meleeHolder.Equals(null)) return;
    var normal = _dotSight.NormalizeFromPoint(meleeHolder.transform.position);
    var angle = 360f - Mathf.Atan2(normal.z, normal.x) * Mathf.Rad2Deg;
    var destRot = Utility.RotateByNormal(normal, Vector3.up);
    var meleeHolderTransform = meleeHolder.transform;
    meleeHolderTransform.rotation = Quaternion.RotateTowards(meleeHolderTransform.rotation, destRot, Time.deltaTime * 630f);
  }

  void KeepInCover(NetMeleeHolder meleeHolder)
  {
    if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals(null))
    {
      meleeHolder.KeepInCover();
    }
  }

  void TakeUpArm(NetMeleeHolder meleeHolder)
  {
    if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals(null))
    {
      meleeHolder.TakeUpArm();
    }
  }

  void HoldTrigger(NetMeleeHolder meleeHolder)
  {
    if (meleeHolder != null && meleeHolder is Object && !meleeHolder.Equals(null))
    {
      meleeHolder.HoldTrigger();
    }
  }

  void ReleaseTrigger(NetMeleeHolder meleeHolder)
  {

  }
}
