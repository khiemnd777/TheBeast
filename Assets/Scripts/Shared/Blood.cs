using UnityEngine;

public class Blood : MonoBehaviour
{
  public new ParticleSystem particleSystem;

  public static Blood BleedOutAtPoint(Blood bloodPrefab, Vector3 normal, Vector3 bleedPoint)
  {
    if (bloodPrefab)
    {
      var rot = 360f - Mathf.Atan2(normal.z, normal.x) * Mathf.Rad2Deg;
      var bloodIns = Object.Instantiate<Blood>(bloodPrefab, bleedPoint, Quaternion.Euler(0f, rot, 0f));
      Object.Destroy(bloodIns.gameObject, bloodIns.particleSystem.main.startLifetimeMultiplier);
      return bloodIns;
    }
    return null;
  }
}
