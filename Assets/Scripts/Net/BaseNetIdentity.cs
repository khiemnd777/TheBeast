using Net.Socket;
using UnityEngine;

namespace Net
{
  public class BaseNetIdentity : MonoBehaviour
  {
    public bool syncCreatingImmediately;
    public string netPrefabName;
    
    protected ISocketWrapper socket;
    protected Settings settings;
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
      socket = NetworkManagerCache.socket;
      if (settings.isServer)
      {
        id = GetInstanceID();
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
      if (!settings.isClient) return;
      this.id = id;
    }
  }
}
