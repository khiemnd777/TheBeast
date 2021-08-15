using UnityEngine;

namespace Net
{
  public class NetBullet : BaseNetIdentity
  {
    /// <summary>
    /// [Readonly] The player identity through network.
    /// </summary>
    public int playerId { get; private set; }

    /// <summary>
    /// [Readonly] This returns true if this object is the one that represents the player on the local machine.
    /// </summary>
    public bool isLocalPlayer { get; private set; }

    [System.NonSerialized]
    public NetWeapon weapon;
    public float damage;
    public float speed;
    public float lifetime;
    protected NetBulletList netBulletList;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake ()
    {
      base.Awake ();
      netBulletList = NetBulletList.instance;
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public virtual void OnTriggerEnter (Collider other)
    {
      var netIdentity = other.GetComponent<NetIdentity> ();
      if (netIdentity && netIdentity.id == playerId) return;

      var hittable = other.GetComponent<IHittable> ();
      if (hittable != null)
      {
        hittable.Hit (transform, damage);
        Destroy (gameObject);
      }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    public virtual void OnDestroy ()
    {
      if (settings.isServer)
      {
        socket.Emit (Constants.EVENT_SERVER_BULLET_REMOVE, new NetIdentityJSON (id));
        netBulletList.Remove (id);
      }
    }

    /// <summary>
    /// Initializes the bullet through some informations.
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    /// <param name="isClient"></param>
    /// <param name="isServer"></param>
    /// <param name="id"></param>
    /// <param name="playerId"></param>
    /// <param name="speed"></param>
    /// <param name="lifetime"></param>
    /// <param name="weapon"></param>
    public void Init (bool isLocalPlayer,
      bool isClient,
      bool isServer,
      int id,
      int playerId,
      float damage,
      float speed,
      float lifetime,
      NetWeapon weapon)
    {
      this.isLocalPlayer = isLocalPlayer;
      this.isClient = isClient;
      this.isServer = isServer;
      this.id = id;
      this.playerId = playerId;
      this.damage = damage;
      this.speed = speed;
      this.lifetime = lifetime;
      this.weapon = weapon;
    }

    /// <summary>
    /// Initializes the bullet through some informations.
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    /// <param name="isClient"></param>
    /// <param name="isServer"></param>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <param name="lifetime"></param>
    /// <param name="weapon"></param>
    public void Init (
      bool isLocalPlayer,
      bool isClient,
      bool isServer,
      float damage,
      float speed,
      float lifetime,
      NetWeapon weapon)
    {
      Init (
        isLocalPlayer,
        isClient,
        isServer,
        this.id,
        this.playerId,
        damage,
        speed,
        lifetime,
        weapon
      );
    }

    public void SetPlayerId (int playerId)
    {
      this.playerId = playerId;
    }
  }
}
