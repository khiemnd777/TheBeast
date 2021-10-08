using Net;
using UnityEngine;

public class DroppedGunController : MonoBehaviour
{
  [SerializeField]
  NetGunHolder _netGunHolder;

  public DroppedGun Drop(Vector3 position, Quaternion rotation, float droppedRadius = 0)
  {
    var droppedGun = _netGunHolder.gun?.droppedGun;
    if (droppedGun)
    {
      var spawnPosition = position + Random.insideUnitSphere * droppedRadius;
      var gun = NetIdentity.InstantiateServerAndEverywhere(droppedGun.prefabName, droppedGun, new Vector3(spawnPosition.x, 0, spawnPosition.z), rotation, null, null, true);
      return gun;
    }
    return null;
  }
}