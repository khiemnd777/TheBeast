using System;
using System.Collections;
using Net.Socket;
using SocketIO;
using UnityEngine;

namespace Net
{
  public class SocketNetworkManager : MonoBehaviour
  {
    /// <summary>
    /// [Readonly] This points out that is the client identity,
    /// it's been established at the init-game.
    /// </summary>
    /// <value></value>
    public Guid clientId { get; private set; }

    Settings settings;

    /// <summary>
    /// Notify that a connection established.
    /// </summary>
    public event Action onConnected;
    public event Action onServerConnected;

    /// <summary>
    /// Notify that a player has registered.
    /// </summary>
    public event Action<NetPlayerJSON> onRegistered;

    /// <summary>
    /// Notify that an other player has registered.
    /// </summary>
    public event Action<NetPlayerJSON> onOtherRegistered;

    /// <summary>
    /// Notify that the loaded player must be init to synchronize with the others.
    /// </summary>
    public event Action<NetLoadedPlayerJSON> onLoadedPlayer;

    /// <summary>
    /// Notify that the list of the players is empty.
    /// </summary>
    public event Action onEmptyListLoading;

    /// <summary>
    /// Notify that an another player has disconnected.
    /// </summary>
    public event Action<NetIdentityJSON> onOtherDisconnected;

    /// <summary>
    /// Notify that an other player is being translated.
    /// </summary>
    public event Action<NetPositionJSON> onOtherPlayerTranslate;

    /// <summary>
    /// Notify that an other player is being rotated.
    /// </summary>
    public event Action<NetRotationJSON> onOtherPlayerRotate;

    /// <summary>
    /// Notify that a specific player was dead at server-side.
    /// </summary>
    public event Action<NetPlayerJSON> onPlayerWasDead;

    /// <summary>
    /// Notify that the specific player's hp will be sync from server-side.
    /// </summary>
    public event Action<NetPlayerJSON> onPlayerSyncHp;

    /// <summary>
    /// Notify that the specific player's max hp will be sync from server-side.
    /// </summary>
    public event Action<NetPlayerJSON> onPlayerSyncMaxHp;

    public event Action<NetRegisterJSON> onRequireRegisterPlayer;
    public event Action<NetRegisterFinishedJSON> onSyncRegisterPlayerFinished;
    public event Action<NetSocketIdJSON> onRequireGettingPlayers;
    public event Action<NetLoadingPlayerJSON> onPlayerFetching;

    /// <summary>
    /// Private of the socket wrapper
    /// </summary>
    ISocketWrapper _socket;

    /// <summary>
    /// The socket wrapper
    /// </summary>
    public ISocketWrapper socket
    {
      get
      {
        return _socket;
      }
    }

    /// <summary>
    /// Sends a registration message to the server to notify that a new player has just been established.
    /// </summary>
    /// <param name="name"></param>
    public void Register(string name)
    {
      _socket.Emit(
        Constants.EVENT_REGISTER,
        new NetRegisterJSON(clientId.ToString(), string.Empty, name)
      );
    }

    /// <summary>
    /// Connected message
    /// </summary>
    /// <param name="evt"></param>
    void OnClientConnected(SocketEvent evt)
    {
      if (settings.isServer)
      {
        print("Server has connected to websocket.");
        if (onServerConnected != null)
        {
          onServerConnected();
        }
        return;
      }
      print("Client has connected to websocket.");
      if (onConnected != null)
      {
        onConnected();
      }
    }

    /// <summary>
    /// Registered message
    /// </summary>
    /// <param name="evt"></param>
    void OnClientRegistered(SocketEvent evt)
    {
      var registeredNetPlayer = NetPlayerJSON.Deserialize(evt.data);
      print("Player has registered");
      print($"Registered player {{name: {registeredNetPlayer.name}, id: {registeredNetPlayer.id}}}");
      if (onRegistered != null)
      {
        onRegistered(registeredNetPlayer);
      }
    }

    /// <summary>
    /// Other registered message
    /// </summary>
    /// <param name="evt"></param>
    void OnClientOtherRegistered(SocketEvent evt)
    {
      var registeredNetPlayer = NetPlayerJSON.Deserialize(evt.data);
      print($"Other player {{name: {registeredNetPlayer.name}, id: {registeredNetPlayer.id}}} has registered");
      if (onRegistered != null)
      {
        onOtherRegistered(registeredNetPlayer);
      }
    }

    /// <summary>
    /// Init players message
    /// </summary>
    /// <param name="evt"></param>
    void OnClientLoadedPlayer(SocketEvent evt)
    {
      var netLoadedPlayerJSON = NetLoadedPlayerJSON.Deserialize(evt.data);
      var netPlayerJSON = netLoadedPlayerJSON.player;
      print($"-- Loaded player {{name: {netPlayerJSON.name}, id: {netPlayerJSON.id}}}");
      if (onLoadedPlayer != null)
      {
        onLoadedPlayer(netLoadedPlayerJSON);
      }
    }

    /// <summary>
    /// Empty list loading message
    /// </summary>
    /// <param name="evt"></param>
    void OnClientEmptyListLoading(SocketEvent evt)
    {
      print("Empty list of the players.");
      if (onEmptyListLoading != null)
      {
        onEmptyListLoading();
      }
    }

    /// <summary>
    /// Other disconnected message
    /// </summary>
    /// <param name="obj"></param>
    void OnClientOtherDisconnected(SocketEvent obj)
    {
      var dataJSON = JsonUtility.FromJson<NetIdentityJSON>(obj.data.ToString());
      if (onOtherDisconnected != null)
      {
        onOtherDisconnected(dataJSON);
      }
    }

    /// <summary>
    /// Other player translate message.
    /// </summary>
    /// <param name="evt"></param>
    void OnOtherPlayerTranslate(SocketEvent evt)
    {
      var netPositionJSON = NetPositionJSON.Deserialize(evt.data);
      if (onOtherPlayerTranslate != null)
      {
        onOtherPlayerTranslate(netPositionJSON);
      }
    }

    /// <summary>
    /// Other player rotate message.
    /// </summary>
    /// <param name="evt"></param>
    void OnOtherPlayerRotate(SocketEvent evt)
    {
      var netRotationJSON = NetRotationJSON.Deserialize(evt.data);
      if (onOtherPlayerRotate != null)
      {
        onOtherPlayerRotate(netRotationJSON);
      }
    }

    void OnRequireRegisterPlayer(SocketEvent evt)
    {
      var dataJSON = JsonUtility.FromJson<NetRegisterJSON>(evt.data.ToString());
      if (onRequireRegisterPlayer != null)
      {
        onRequireRegisterPlayer(dataJSON);
      }
    }

    void OnRequireGettingPlayers(SocketEvent evt)
    {
      var dataJSON = JsonUtility.FromJson<NetSocketIdJSON>(evt.data.ToString());
      if (onRequireGettingPlayers != null)
      {
        onRequireGettingPlayers(dataJSON);
      }
    }

    void OnPlayerFetching(SocketEvent evt)
    {
      var dataJSON = JsonUtility.FromJson<NetLoadingPlayerJSON>(evt.data.ToString());
      if (onPlayerFetching != null)
      {
        onPlayerFetching(dataJSON);
      }
    }

    void OnClientPlayerDead(SocketEvent evt)
    {
      var netPlayerJSON = JsonUtility.FromJson<NetPlayerJSON>(evt.data.ToString());
      if (onPlayerWasDead != null)
      {
        onPlayerWasDead(netPlayerJSON);
      }
    }

    void OnClientPlayerSyncHp(SocketEvent evt)
    {
      var netPlayerJSON = JsonUtility.FromJson<NetPlayerJSON>(evt.data.ToString());
      if (onPlayerSyncHp != null)
      {
        onPlayerSyncHp(netPlayerJSON);
      }
    }

    void OnClientPlayerSyncMaxHp(SocketEvent evt)
    {
      var netPlayerJSON = JsonUtility.FromJson<NetPlayerJSON>(evt.data.ToString());
      if (onPlayerSyncMaxHp != null)
      {
        onPlayerSyncMaxHp(netPlayerJSON);
      }
    }

    /// <summary>
    /// Connect to server
    /// </summary>
    public IEnumerator ConnectToServer()
    {
      yield return new WaitForSeconds(.5f);
      // emit to server to know the connection.
      _socket.Emit(Constants.EVENT_CONNECT_TO_SERVER, new NetConnectionJSON(settings.isServer));
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
      clientId = Guid.NewGuid();
      _socket = new SocketIOWrapper(FindObjectOfType<SocketIOComponent2>());
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      settings = Settings.instance;

      // Register socket's events.
      _socket.On(Constants.EVENT_CONNECTED, OnClientConnected);
      _socket.On(Constants.EVENT_CLIENT_REGISTERED, OnClientRegistered);
      _socket.On(Constants.EVENT_CLIENT_OTHER_REGISTERED, OnClientOtherRegistered);
      _socket.On(Constants.EVENT_CLIENT_LOADED_PLAYER, OnClientLoadedPlayer);
      _socket.On(Constants.EVENT_CLIENT_EMPTY_LIST, OnClientEmptyListLoading);
      _socket.On(Constants.EVENT_CLIENT_OTHER_DISCONNECTED, OnClientOtherDisconnected);
      _socket.On(Constants.EVENT_CLIENT_PLAYER_TRANSLATE, OnOtherPlayerTranslate);
      _socket.On(Constants.EVENT_CLIENT_PLAYER_ROTATE, OnOtherPlayerRotate);
      _socket.On(Constants.EVENT_CLIENT_PLAYER_DEAD, OnClientPlayerDead);
      _socket.On(Constants.EVENT_CLIENT_PLAYER_SYNC_HP, OnClientPlayerSyncHp);
      _socket.On(Constants.EVENT_CLIENT_PLAYER_SYNC_MAX_HP, OnClientPlayerSyncMaxHp);
      // If the game runs on the server-side, then connect to websocket immediately.
      if (settings.isServer)
      {
        _socket.On(Constants.EVENT_REQUIRE_REGISTER_PLAYER, OnRequireRegisterPlayer);
        _socket.On(Constants.EVENT_REQUIRE_GETTING_PLAYERS, OnRequireGettingPlayers);
        print("Connecting to websocket...");
        StartCoroutine(ConnectToServer());
      }
      else
      {
        _socket.On(Constants.EVENT_DOWNLOAD_PLAYERS, OnPlayerFetching);
        print("Connecting to server...");
        StartCoroutine(ConnectToServer());
      }
    }
  }
}
