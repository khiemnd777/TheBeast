using System;
using Net.Socket;
using UnityEngine;

namespace Net
{
  [RequireComponent (typeof (NetPlayerList))]
  public class NetTransformSynchronizer : MonoBehaviour
  {
    Settings settings;
    NetPlayerList netPlayerList;
    NetBulletList netBulletList;
    SocketNetworkManager socketNetworkManager;
    ISocketWrapper socket;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start ()
    {
      settings = Settings.instance;
      netPlayerList = NetPlayerList.instance;
      netBulletList = NetBulletList.instance;
      socketNetworkManager = SocketNetworkManagerCache.GetInstance ();
      socket = SocketNetworkManagerCache.socket;
      // Init events.
      socketNetworkManager.onPlayerWasDead += OnPlayerWasDead;
      socketNetworkManager.onPlayerSyncHp += OnPlayerSyncHp;
      socketNetworkManager.onPlayerSyncMaxHp += OnPlayerSyncMaxHp;
      socketNetworkManager.onRequireGettingPlayers += OnRequireGettingPlayers;
    }

    void OnRequireGettingPlayers (NetSocketIdJSON netSocketIdJSON)
    {
      var list = netPlayerList.All ();
      list.ForEach (netIdentity =>
      {
        if (!netIdentity) return;
        var player = netIdentity.GetComponent<Player> ();
        socket.Emit (Constants.EVENT_RESPONSE_GETTING_PLAYERS, new NetLoadingPlayerJSON (
          netSocketIdJSON.socketId,
          netIdentity.netName,
          netIdentity.id,
          Utility.Vector3ToPositionArray (netIdentity.transform.position),
          player.life,
          player.maxLife
        ));
      });
    }

    /// <summary>
    /// Notify that the specific player's hp will be sync from server-side.
    /// </summary>
    /// <param name="netPlayerJSON"></param>
    void OnPlayerSyncHp (NetPlayerJSON netPlayerJSON)
    {
      var netIdentity = netPlayerList.Find (netPlayerJSON.id);
      if (netIdentity)
      {
        var player = netIdentity.GetComponent<Player> ();
        if (player)
        {
          player.SetHp (netPlayerJSON.hp);
        }
      }
    }

    /// <summary>
    /// Notify that the specific player's max hp will be sync from server-side.
    /// </summary>
    /// <param name="dataJSON"></param>
    void OnPlayerSyncMaxHp (NetPlayerJSON dataJSON)
    {
      var netIdentity = netPlayerList.Find (dataJSON.id);
      if (netIdentity)
      {
        var player = netIdentity.GetComponent<Player> ();
        if (player)
        {
          player.SetHp (dataJSON.hp);
          player.SetMaxHp (dataJSON.maxHp);
        }
      }
    }

    /// <summary>
    /// Notify that a specific player was dead at server-side.
    /// </summary>
    /// <param name="netPlayerJSON"></param>
    void OnPlayerWasDead (NetPlayerJSON netPlayerJSON)
    {
      var netIdentity = netPlayerList.Find (netPlayerJSON.id);
      if (netIdentity)
      {
        netPlayerList.Remove (netIdentity.id);
        Destroy (netIdentity.gameObject);
      }
    }
  }
}
