using UnityEngine;

public class NetRifle : NetGun
{
  [SerializeField]
  Heat _heat;

  [SerializeField]
  Animator _fireAnim;

  public override void OnTriggerEffect()
  {
    EjectShell();
    flashAnim.Play("Gun Flash", 0, 0);
    _fireAnim.Play("Rifle Fire", 0, 0);
    audioSource.Play();
    base.OnTriggerEffect();
  }

  public override void TakeUpArm()
  {
    base.TakeUpArm();
    if (netIdentity.isLocal)
    {
      var fov = this.GetFieldOfView(0);
      
      cameraController.SetTarget(player.transform);
      cameraController.SetDefaultLimitedBoundingBox();

      player.fieldOfView.SetRadius(fov.radius);
      player.fieldOfView.SetAngle(fov.angle);
      
      dotSightController.ResetSensitivity();
      dotSightController.SetLocally();
      
      heatUI.Visible(true);
      heatUI.SetHeat(_heat);
    }
  }

  public override void KeepInCover()
  {
    heatUI.Visible(false);
    base.KeepInCover();
  }

  public override void OnSecondAction()
  {
    if (netIdentity.isLocal)
    {
      var fovIndex = this.SwitchFieldOfViewIndex();
      var fov = this.GetFieldOfView(fovIndex);
      if (fovIndex > 0)
      {
        cameraController.SetTarget(dotSight.transform);
        cameraController.SetLimitedBoundingBox(new BoundingBox
        {
          size = new Vector3(fov.radius * 2, 0f, fov.radius * 2),
          centerTarget = player.transform
        });

        dotSightController.SetSensitivity(.25f);
        dotSightController.SetGlobally();

      }
      else
      {
        cameraController.SetTarget(player.transform);
        cameraController.SetDefaultLimitedBoundingBox();

        dotSightController.ResetSensitivity();
        dotSightController.SetLocally();
      }

      player.fieldOfView.SetRadius(fov.radius);
      player.fieldOfView.SetAngle(fov.angle);
    }
  }
}
