using UnityEngine;

namespace Net
{
  [RequireComponent (typeof (SocketNetworkManager))]
  public class NetBulletList : BaseNetList<NetBullet>
  {
    [SerializeField]
    NetBullet netBulletPrefab;

    static NetBulletList pInstance;

    public static NetBulletList instance
    {
      get
      {
        return pInstance ?? (pInstance = FindObjectOfType<NetBulletList> ());
      }
    }

    /// <summary>
    /// Instatiates the bullet at the server-side after it was being launched from the projectile.
    /// </summary>
    /// <param name="netBulletPrefab"></param>
    /// <param name="playerId"></param>
    /// <param name="projectilePosition"></param>
    /// <param name="projectileRotation"></param>
    /// <returns></returns>
    public NetBullet CreateBullet (NetBullet netBulletPrefab, int playerId, Vector3 projectilePosition, Quaternion projectileRotation)
    {
      var bullet = Instantiate<NetBullet> (netBulletPrefab, projectilePosition, projectileRotation);
      bullet.SetPlayerId (playerId);
      return bullet;
    }
    
    /// <summary>
    /// Instatiates the bullet at the client-side after it was emitted from the server-side.
    /// </summary>
    /// <param name="netBulletPrefab"></param>
    /// <param name="id"></param>
    /// <param name="playerId"></param>
    /// <param name="projectilePosition"></param>
    /// <param name="projectileRotation"></param>
    /// <returns></returns>
    public NetBullet CreateBullet (NetBullet netBulletPrefab, int id, int playerId, Vector3 projectilePosition, Quaternion projectileRotation)
    {
      var bullet = Instantiate<NetBullet> (netBulletPrefab, projectilePosition, projectileRotation);
      bullet.SetNetIdAtClientSide (id);
      bullet.SetPlayerId (playerId);
      return bullet;
    }
  }
}
