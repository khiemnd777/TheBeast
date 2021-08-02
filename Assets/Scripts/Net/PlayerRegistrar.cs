using System;
using Net.Socket;
using UnityEngine;

namespace Net
{
  public class PlayerRegistrar : MonoBehaviour
  {
    public event Action<int, int> onPlayerCount;
    public event Action onLocalPlayerRegistered;
    [NonSerialized]
    public string registeredPlayerName;
    [SerializeField]
    NetIdentity gamePlayerPrefab;
    [SerializeField]
    SocketNetworkManager socketNetworkManager;
    ISocketWrapper socket;
    NetPlayerList netPlayerList;
    CameraController cameraController;
    DotSightController dotSightController;
    OtherPlayerLoadingCounter otherPlayerCounter;
    Settings settings;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      settings = Settings.instance;
      socketNetworkManager = SocketNetworkManagerCache.GetInstance();
      socket = SocketNetworkManagerCache.socket;
      netPlayerList = NetPlayerList.instance;
      otherPlayerCounter = new OtherPlayerLoadingCounter();
      dotSightController = FindObjectOfType<DotSightController>();
      cameraController = FindObjectOfType<CameraController>();
      InitEvents();
    }

    void InitEvents()
    {
      // This will be fired after the connection was completely connected.
      socketNetworkManager.onConnected += () =>
      {
        print("Initializing other players...");
        socket.Emit(Constants.EVENT_SERVER_LOAD_PLAYERS);
      };
      // This will be fired after a registration was registered.
      socketNetworkManager.onRegistered += (netPlayerJSON) =>
      {
        // Create the player by the prefab through the JSON data.
        CreatePlayer(netPlayerJSON.id, netPlayerJSON.name, netPlayerJSON.hp);
      };
      // This will be fired to notify to local machine about the other player was registered.
      socketNetworkManager.onOtherRegistered += (netPlayerJSON) =>
      {
        // Create the another player by the prefab through the JSON data.
        CreateOtherPlayer(netPlayerJSON.id, netPlayerJSON.name, netPlayerJSON.hp);
      };
      // This will be fired to notify to local machine must create another players to synchronize with the others.
      socketNetworkManager.onLoadedPlayer += (netLoadedPlayerJSON) =>
      {
        var netPlayerJSON = netLoadedPlayerJSON.player;
        // Create the other players in the list that be sent down through the server.
        CreateOtherPlayer(netPlayerJSON.id, netPlayerJSON.name, netPlayerJSON.hp);
        // Counter counts.
        var total = netLoadedPlayerJSON.total;
        otherPlayerCounter.Count(total, (count) =>
       {
         if (onPlayerCount != null)
         {
           onPlayerCount(count, total);
         }
       });
      };
      // This will be fired to notify that the list is empty, then register the local player.
      socketNetworkManager.onEmptyListLoading += () =>
      {
        // RegisterPlayer ();
      };
      // This will be fired when the player loading was finished.
      otherPlayerCounter.onFinishedLoading += () =>
      {
        // RegisterPlayer ();
      };

      if (settings.isServer)
      {
        socketNetworkManager.onRequireRegisterPlayer += (NetRegisterJSON netRegisterJSON) =>
        {
          print($"Register '{netRegisterJSON.netName}' for client-id '{netRegisterJSON.clientId}'");
          CreatePlayerAtTheServerSide(
            netRegisterJSON.clientId,
            netRegisterJSON.netName,
            300f
          );
        };
      }
      else
      {
        socketNetworkManager.onSyncRegisterPlayerFinished += (NetRegisterFinishedJSON netRegisterFinishedJSON) =>
        {
          // Detects the local player at the client-side.
          if (netRegisterFinishedJSON.clientId.Equals(socketNetworkManager.clientId.ToString()))
          {
            if (onLocalPlayerRegistered != null)
            {
              onLocalPlayerRegistered();
            }
          }
          CreatePlayerAtTheClientSide(
            netRegisterFinishedJSON.clientId,
            netRegisterFinishedJSON.playerName,
            netRegisterFinishedJSON.id,
            netRegisterFinishedJSON.position,
            netRegisterFinishedJSON.hp,
            netRegisterFinishedJSON.hp
          );
        };

        socketNetworkManager.onPlayerFetching += (NetLoadingPlayerJSON netLoadingPlayerJSON) =>
        {
          CreatePlayerAtTheClientSide(
            Guid.Empty.ToString(),
            netLoadingPlayerJSON.playerName,
            netLoadingPlayerJSON.id,
            netLoadingPlayerJSON.position,
            netLoadingPlayerJSON.hp,
            netLoadingPlayerJSON.maxHp
          );
        };
      }
    }

    /// <summary>
    /// Register player that one presents on the local machine.
    /// </summary>
    public void RegisterPlayer()
    {
      if (settings.isServer) return;
      print("Registering...");
      socketNetworkManager.Register(registeredPlayerName);
    }

    public void RegisterPlayer(string playerName)
    {
      if (settings.isServer) return;
      print("Registering...");
      registeredPlayerName = playerName;
      socketNetworkManager.Register(registeredPlayerName);
    }

    /// <summary>
    /// Create net player by prefab.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="hp"></param>
    void CreatePlayer(int id, string name, float hp)
    {
      if (netPlayerList.Exists(id)) return;
      var netIdentityIns = Instantiate<NetIdentity>(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
      netIdentityIns.Init(id, name);
      netPlayerList.Store(netIdentityIns);
      cameraController.SetTarget(netIdentityIns.transform);
      dotSightController.InitDotSight();
      dotSightController.SetPlayer(netIdentityIns);
      // Set health point to the player.
      var player = netIdentityIns.GetComponent<Player>();
      if (player)
      {
        player.SetHp(hp);
      }
    }

    /// <summary>
    /// Create other net player by prefab.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void CreateOtherPlayer(int id, string name, float hp)
    {
      if (netPlayerList.Exists(id)) return;
      var gamePlayer = Instantiate<NetIdentity>(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
      gamePlayer.InitOther(id, name);
      netPlayerList.Store(gamePlayer);
      // Set health point to the player.
      var player = gamePlayer.GetComponent<Player>();
      if (player)
      {
        player.SetHp(hp);
      }
    }

    void CreatePlayerAtTheServerSide(string clientId, string playerName, float hp)
    {
      if (!settings.isServer) return;
      var player = netPlayerList.CreatePlayer(gamePlayerPrefab, playerName, hp, Vector3.zero);
      netPlayerList.Store(player);
      Debug.Log($"The player '{playerName}' '{player.id}' has been instantiated.");
      socket.Emit(Constants.EVENT_CLIENT_REGISTER_PLAYER_FINISHED,
        new NetRegisterFinishedJSON(
          clientId,
          playerName,
          player.id,
          Utility.Vector3ToPositionArray(player.transform.position),
          hp
        )
      );
    }

    void CreatePlayerAtTheClientSide(string clientId, string playerName, int id, float[] position, float hp, float maxHp)
    {
      if (!settings.isClient) return;
      var isLocalPlayer = clientId.Equals(socketNetworkManager.clientId.ToString());
      var player = netPlayerList.CreatePlayer(
        gamePlayerPrefab,
        id,
        playerName,
        hp,
        maxHp,
        Utility.PositionArrayToVector3(Vector3.zero, position),
        isLocalPlayer
      );
      if (player.isLocal)
      {
        Debug.Log($"The player '{playerName}' '{id}' has been instantiated.");
        cameraController.SetTarget(player.transform);
        dotSightController.InitDotSight();
        dotSightController.SetPlayer(player);
        socket.Emit(Constants.EVENT_PLAYER_STORE_ID, new NetIdentityJSON(id));
      }
      netPlayerList.Store(player);
    }
  }
}
