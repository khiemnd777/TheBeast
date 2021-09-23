using System;
using UnityEngine;

namespace Net
{
  public class NetIdentity : BaseNetIdentity
  {
    public event Action<string, string> onMessageReceived;

    public event Action<string> onClone;

    /// <summary>
    /// This property is used at server-side and local-site.
    /// </summary>
    public string clientId { get; set; }

    /// <summary>
    /// This property is used at server-side.
    /// </summary>
    public string type { get; set; }

    public string prefabName { get; set; }

    public float life { get; set; }

    protected float currentLife { get; set; }

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

    public void CloneEverywhereImmediately(string prefabName, float lifetime, object otherMessage)
    {
      if (isLocal)
      {
        var clientId = networkManager.clientId;
        socket.Emit(Constants.EVENT_CLONE_EVERYWHERE, new NetCloneJSON(
            clientId.ToString(),
            prefabName,
            name,
            life,
            maxLife,
            lifetime,
            Point.FromVector3(transform.position),
            transform.rotation,
            otherMessage
          )
        );
      }
    }

    public void SetIsLocal()
    {
      this.isLocal = true;
    }

    public void EmitMessage(string eventName, object message, bool onlyServer = false)
    {
      var clientId = NetworkManagerCache.networkManager.clientId;
      socket.Emit(Constants.EVENT_EMIT_MESSAGE, new NetMessageJSON(
        clientId
        , id
        , prefabName
        , netName
        , life
        , maxLife
        , Utility.Vector3ToPositionArray(transform.position)
        , Utility.QuaternionToAnglesArray(transform.rotation)
        , eventName
        , message
        , onlyServer
        )
      );
    }

    public virtual void OnCloneMessage(string otherMessage)
    {
      if (onClone != null)
      {
        onClone(otherMessage);
      }
    }

    public virtual void OnReceiveMessage(string eventName, string message)
    {
      if (onMessageReceived != null)
      {
        onMessageReceived(eventName, message);
      }
    }

    public static T InstantiateLocal<T>(T original, Vector3 position, Quaternion rotation) where T : NetIdentity
    {
      var target = Instantiate<T>(original, position, rotation);
      target.SetIsLocal();
      return target;
    }

    public static T InstantiateLocalAndEverywhere<T>(string prefabName, T original, Vector3 position, Quaternion rotation, Func<T, float> funcLifetime, object otherMessage) where T : NetIdentity
    {
      var target = InstantiateLocal(original, position, rotation);
      var lifetime = funcLifetime != null ? funcLifetime(target) : 0f;
      target.CloneEverywhereImmediately(prefabName, lifetime, otherMessage);
      if (lifetime > 0f)
      {
        Destroy(target.gameObject, lifetime);
      }
      return target;
    }
  }
}
