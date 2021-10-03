using Net.Socket;
using UnityEngine;

namespace Net
{
  public class BaseNetIdentity : MonoBehaviour
  {
    protected NetworkManager networkManager;
    protected ISocketWrapper socket;
    protected Settings settings;
    protected NetObjectList netObjectList;
    /// <summary>
    /// [Readonly] The player identity through network.
    /// </summary>
    public int id { get; protected set; }

    /// <summary>
    /// [Readonly] Returns true if running as a client and this object was spawned by a server.
    /// </summary>
    public bool isClient { get; protected set; }

    /// <summary>
    /// [Readonly] Returns true if this object is active on an active server.
    /// </summary>
    public bool isServer { get; protected set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
      // Init the Settings through a static instance.
      settings = Settings.instance;
      networkManager = NetworkManagerCache.networkManager;
      socket = NetworkManagerCache.socket;
      netObjectList = NetObjectList.instance;

      if (settings.isServer)
      {
        id = GetInstanceID();
        isServer = true;
      }
      if (settings.isClient)
      {
        isClient = true;
      }
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
    }

    public void SetNetIdAtClientSide(int id)
    {
      if (settings.isClient)
      {
        this.id = id;
      }
    }

    public void SetNetIdAtServerSide(int id)
    {
      if (settings.isServer)
      {
        this.id = id;
      }
    }

    public void NetDestroy(NetIdentity netId)
    {
      if (netObjectList)
      {
        netObjectList.Remove(netId.id);
      }
      Destroy(netId.gameObject);
    }
  }
}
