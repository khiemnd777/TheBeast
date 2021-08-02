using UnityEngine;

namespace Net
{
  public class NetWeaponRecoil : MonoBehaviour
  {
    NetWeaponController netWeaponController;
    Rigidbody rb;
    Transform cachedTransform;
    Transform cachedProjectilePoint;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake ()
    {
      rb = GetComponent<Rigidbody> ();
      netWeaponController = GetComponent<NetWeaponController> ();
      cachedProjectilePoint = netWeaponController.weapon.projectilePoint;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start ()
    {
      cachedTransform = transform;
    }

    public void Recoil (Vector3 projectilePoint, float forceVal = 1f)
    {
      var direction = projectilePoint - cachedTransform.position;
      direction.Normalize ();
      rb.AddForce (-direction * forceVal, ForceMode.Impulse);
    }
  }
}
