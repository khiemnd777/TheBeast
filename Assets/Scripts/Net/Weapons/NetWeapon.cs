using Net.Socket;
using UnityEngine;

namespace Net
{
  public abstract class NetWeapon : MonoBehaviour
  {
    protected Settings settings;
    protected SocketNetworkManager socketNetworkManager;
    protected ISocketWrapper socket;
    protected NetWeaponRecoil weaponRecoil;
    [System.NonSerialized]
    public Player player;
    [System.NonSerialized]
    public NetIdentity netIdentity;
    public NetBullet bulletPrefab;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletLifetime;
    public float interval;
    public Transform projectilePoint;
    [System.NonSerialized]
    public NetBulletList netBulletList;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public virtual void Awake ()
    {
      socketNetworkManager = SocketNetworkManagerCache.GetInstance ();
      socket = SocketNetworkManagerCache.socket;
      netBulletList = NetBulletList.instance;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public virtual void Start ()
    {
      settings = Settings.instance;
      weaponRecoil = player.GetComponent<NetWeaponRecoil> ();
    }

    /// <summary>
    /// Executes the projectile object.
    /// </summary>
    public virtual void HoldTrigger ()
    {

    }

    /// <summary>
    /// Create the instance of bullet.
    /// </summary>
    /// <returns></returns>
    public NetBullet CreateBullet (int id, int playerId, float[] position, float[] rotation)
    {
      if (!bulletPrefab) return null;
      if (!settings.isClient) return null;
      var bullet = netBulletList.CreateBullet (
        bulletPrefab,
        id,
        playerId,
        Utility.PositionArrayToVector3 (projectilePoint.position, position),
        Utility.AnglesArrayToQuaternion (rotation)
      );
      netBulletList.Store (bullet);
      // Initializes the bullet.
      bullet.Init (
        netIdentity.isLocal,
        netIdentity.isClient,
        netIdentity.isServer,
        bulletDamage,
        bulletSpeed,
        bulletLifetime,
        this
      );
      return bullet;
    }
  }
}
