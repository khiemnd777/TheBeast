using Net.Socket;
using UnityEngine;

namespace Net
{
  public class BaseNetIdentity : MonoBehaviour
  {
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
      OnNetAwake();
    }

    protected virtual void OnNetAwake()
    {

    }

    protected virtual void Start()
    {
      OnNetStart();
    }

    protected virtual void OnNetStart()
    {

    }

    protected virtual void Update()
    {
      OnNetUpdate();
    }

    protected virtual void OnNetUpdate()
    {

    }

    protected virtual void FixedUpdate()
    {
      OnNetFixedUpdate();
    }

    protected virtual void OnNetFixedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {
      OnNetLateUpdate();
    }

    protected virtual void OnNetLateUpdate()
    {

    }

    public void SetNetIdAtClientSide(int id)
    {
      if (!settings.isClient) return;
      this.id = id;
    }
  }
}
