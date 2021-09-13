using UnityEngine;

public class NetRifle : NetGun
{
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

  public override void OnSecondAction()
  {
    if (netIdentity.isLocal)
    {
      var fovIndex = this.SwitchFieldOfViewIndex();
      var fov = this.GetFieldOfView(fovIndex);
      if (fovIndex > 0)
      {
        cameraController.SetTarget(dotSight.transform);
        dotSightController.SetSensitivity(.25f);
				dotSightController.SetGlobally();
      }
      else
      {
        cameraController.SetTarget(player.transform);
    		dotSightController.ResetSensitivity();
				dotSightController.SetLocally();
      }
      cameraController.SetLimitedBoundingBox(new BoundingBox
      {
        size = new Vector3(fov.radius * 2, 0f, fov.radius * 2),
        centerTarget = player.transform
      });
    }
  }
}
