using System;

namespace Net
{
  public class NetIdentity : BaseNetIdentity
  {
    public float life { get; set; }

    public float maxLife { get; set; }

    public bool lifeEnd
    {
      get
      {
        return life <= 0;
      }
    }

    /// <summary>
    /// This is fired after init has done.
    /// </summary>
    public event Action onAfterInit;

    /// <summary>
    /// [Readonly] The player name through network.
    /// </summary>
    public string netName { get; private set; }

    /// <summary>
    /// [Readonly] This returns true if this object is the one that represents the player on the local machine.
    /// </summary>
    public bool isLocal { get; private set; }

    /// <summary>
    /// Init player after instantiating.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void Init(int id, string name)
    {
      InitBy(true, id, name);
    }

    /// <summary>
    /// Init another player after instantiating.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void InitOther(int id, string name)
    {
      InitBy(false, true, id, name);
    }

    /// <summary>
    /// Init server player after instantiating.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void InitServer(int id, string name)
    {
      InitBy(false, false, true, id, name);
    }

    /// <summary>
    /// Init the player that being played on the local machine or not.
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void InitBy(bool isLocalPlayer, int id, string name)
    {
      InitBy(isLocalPlayer, true, id, name);
    }

    /// <summary>
    /// Init the player that being played on the local machine or not.
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    /// <param name="isClient"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void InitBy(bool isLocalPlayer, bool isClient, int id, string name)
    {
      InitBy(isLocalPlayer, isClient, false, id, name);
    }

    /// <summary>
    /// Init the player that being played on the local machine or not.
    /// </summary>
    /// <param name="isLocalPlayer"></param>
    /// <param name="isClient"></param>
    /// <param name="isServer"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void InitBy(bool isLocalPlayer, bool isClient, bool isServer, int id, string name)
    {
      this.isLocal = isLocalPlayer;
      this.isClient = isClient;
      this.isServer = isServer;

      SetInformation(id, name);

      if (onAfterInit != null)
      {
        onAfterInit();
      }
    }

    /// <summary>
    /// Set player information.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void SetInformation(int id, string name)
    {
      this.id = id;
      this.netName = name;
    }

    public void EmitMessage(string eventName, object message)
    {
      var clientId = NetworkManagerCache.networkManager.clientId.ToString();
      socket.Emit(Constants.EVENT_EMIT_MESSAGE, new NetMessageJSON(clientId, id, eventName, message));
    }

    public virtual void OnReceiveMessage(string eventName, object message)
    {

    }
  }
}
