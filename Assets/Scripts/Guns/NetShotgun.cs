using Net;
using UnityEngine;

public class NetShotgun : NetGun
{
  [SerializeField]
  Animator _shotgunFireAnim;

  public int ballNumber;

  protected override void InstantiateBullet(string netBulletPrefabName, Vector3 dotSightPoint)
  {
    for (int inx = 0; inx < ballNumber; inx++)
    {
      var angle = 10f;
      var rot = projectile.rotation;
      var rotAngle = rot.eulerAngles;
      var bulletRot = Quaternion.Euler(rotAngle.x, rotAngle.y + Random.Range(-angle, angle), rotAngle.z);
      NetIdentity.InstantiateLocalAndEverywhere<NetBullet>(netBulletPrefabName, bulletPrefab, projectile.position, bulletRot, (netBullet) =>
      {
        return netBullet.CalculateBulletLifetime(dotSightPoint, projectile.position);
      }, new NetBulletCloneJson
      {
        playerNetId = player.id
      });
    }
  }

  public override void OnTriggerEffect()
  {
    EjectShell();
    flashAnim.Play("Gun Flash", 0, 0);
    _shotgunFireAnim.Play("Shotgun Fire", 0, 0);
    audioSource.Play();
    base.OnTriggerEffect();
  }
}
