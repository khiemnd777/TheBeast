using UnityEngine;

namespace Net
{
  [RequireComponent(typeof(NetworkManager))]
  public class NetObjectList : BaseNetList<NetIdentity>
  {
    static NetObjectList pInstance;
    public static NetObjectList instance
    {
      get
      {
        return pInstance ?? (pInstance = FindObjectOfType<NetObjectList>());
      }
    }

    NetworkManager _networkManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    protected override void Start()
    {
      base.Start();
      _networkManager = NetworkManagerCache.networkManager;
      _networkManager.onOtherDisconnected += OnOtherDisconnected;
    }

    /// <summary>
    /// Notify that an another player has disconnected.
    /// </summary>
    /// <param name="dataJSON"></param>
    void OnOtherDisconnected(NetObjectJSON dataJSON)
    {
      var playerId = dataJSON.id;
      // Get matched player by id then destroy him and remove out of the list.
      var matchedPlayer = Find(playerId);
      if (matchedPlayer)
      {
        Remove(playerId);
        Destroy(matchedPlayer.gameObject);
      }
    }

    /// <summary>
    /// Instantiates the player at the server-side.
    /// </summary>
    /// <param name="netIdPrefab"></param>
    /// <param name="netName"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public NetIdentity Create(NetIdentity netIdPrefab,
      string netName,
      Vector3 spawnPosition,
      Quaternion rotation
    )
    {
      var insNetId = Instantiate<NetIdentity>(netIdPrefab, spawnPosition, rotation);
      insNetId.InitServer(insNetId.id, netName);
      return insNetId;
    }

    /// <summary>
    /// Instantiates the player at the client-side.
    /// </summary>
    /// <param name="netPlayerPrefab"></param>
    /// <param name="id"></param>
    /// <param name="netName"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public NetIdentity Create(NetIdentity netPlayerPrefab,
      int id,
      string netName,
      float life,
      float maxLife,
      Vector3 spawnPosition,
      Quaternion rotation,
      bool isLocal
    )
    {
      var insNetId = Instantiate<NetIdentity>(netPlayerPrefab, spawnPosition, rotation);
      insNetId.SetNetIdAtClientSide(id);
      if (isLocal)
      {
        insNetId.Init(insNetId.id, netName);
      }
      else
      {
        insNetId.InitOther(insNetId.id, netName);
      }
      insNetId.life = life;
      insNetId.maxLife = maxLife;
      return insNetId;
    }
  }
}
