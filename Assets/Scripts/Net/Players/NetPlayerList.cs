using UnityEngine;

namespace Net
{
  [RequireComponent (typeof (SocketNetworkManager))]
  public class NetPlayerList : BaseNetList<NetIdentity>
  {
    static NetPlayerList pInstance;
    public static NetPlayerList instance
    {
      get
      {
        return pInstance ?? (pInstance = FindObjectOfType<NetPlayerList> ());
      }
    }

    SocketNetworkManager _socketNetworkManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    protected override void Start ()
    {
      base.Start ();
      _socketNetworkManager = SocketNetworkManagerCache.GetInstance ();
      _socketNetworkManager.onOtherDisconnected += OnOtherDisconnected;
    }

    /// <summary>
    /// Notify that an another player has disconnected.
    /// </summary>
    /// <param name="dataJSON"></param>
    void OnOtherDisconnected (NetIdentityJSON dataJSON)
    {
      var playerId = dataJSON.id;
      // Get matched player by id then destroy him and remove out of the list.
      var matchedPlayer = Find (playerId);
      if (matchedPlayer)
      {
        Remove (playerId);
        Destroy (matchedPlayer.gameObject);
      }
    }

    /// <summary>
    /// Instantiates the player at the server-side.
    /// </summary>
    /// <param name="netPlayerPrefab"></param>
    /// <param name="playerName"></param>
    /// <param name="hp"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public NetIdentity CreatePlayer (NetIdentity netPlayerPrefab,
      string playerName,
      float hp,
      Vector3 spawnPosition
    )
    {
      var insPlayer = Instantiate<NetIdentity> (netPlayerPrefab, spawnPosition, Quaternion.identity);
      insPlayer.InitServer (insPlayer.id, playerName);
      var player = insPlayer.GetComponent<Player> ();
      if (player)
      {
        player.SetHp (hp);
        player.SetMaxHp (hp);
      }
      return insPlayer;
    }

    /// <summary>
    /// Instantiates the player at the client-side.
    /// </summary>
    /// <param name="netPlayerPrefab"></param>
    /// <param name="id"></param>
    /// <param name="playerName"></param>
    /// <param name="hp"></param>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    public NetIdentity CreatePlayer (NetIdentity netPlayerPrefab,
      int id,
      string playerName,
      float hp,
      float maxHp,
      Vector3 spawnPosition,
      bool isClientIdMatched
    )
    {
      var insPlayer = Instantiate<NetIdentity> (netPlayerPrefab, spawnPosition, Quaternion.identity);
      insPlayer.SetNetIdAtClientSide (id);
      if (isClientIdMatched)
      {
        insPlayer.Init (insPlayer.id, playerName);
      }
      else
      {
        insPlayer.InitOther (insPlayer.id, playerName);
      }
      var player = insPlayer.GetComponent<Player> ();
      if (player)
      {
        player.SetHp (hp);
        player.SetMaxHp (maxHp);
      }
      return insPlayer;
    }
  }
}
