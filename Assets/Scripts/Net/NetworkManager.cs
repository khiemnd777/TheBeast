using System;
using System.Collections;
using Net.Socket;
using SocketIO;
using UnityEngine;

namespace Net
{
  public class NetworkManager : MonoBehaviour
  {
    public string clientId { get; private set; }
    public event Action onClientConnected;
    public event Action onServerConnected;
    public event Action<NetObjectJSON> onObjectRegister;
    public event Action<NetObjectJSON> onObjectRegistered;
    public event Action<NetObjectJSON> onOtherObjectRegistered;
    public event Action<NetObjectJSON> onOtherDisconnected;
    public event Action<NetObjectJSON> onClientRegisterFinished;
    public event Action<NetRegisterJSON> onServerRegister;
    public event Action<NetMessageJSON> onReceiveMessageJson;
    public event Action<NetCloneJSON> onBroadcastCloneEverywhereJson;
    public event Action<NetServerCloneJSON> onBroadcastServerCloneEverywhereJson;
    public event Action<NetClientIdJSON> onRequestGettingPlayers;
    public event Action onServerDisconnected;
    public event Action<ScoreJson> onScoreBroadcast;

    Settings _settings;
    ISocketWrapper _socket;
    public ISocketWrapper socket
    {
      get
      {
        return _socket;
      }
    }

    [Obsolete("Use ServerConnect or ClientConnect functions instead.")]
    public IEnumerator Connect()
    {
      yield return new WaitForSeconds(.5f);
      // emit to server to know the connection.
      _socket.Emit(Constants.EVENT_CONNECT, new NetConnectionJSON(_settings.isServer));
    }

    IEnumerator ServerConnect()
    {
      if (_settings.isServer)
      {
        yield return new WaitForSeconds(.5f);
        // Emit an event to server to know the connection from the server-side.
        // Include the room identity, let's the rooms division manager knows the room has been generated.
        _socket.Emit(Constants.EVENT_SERVER_CONNECT, new NetServerConnectJSON
        {
          roomId = Utility.ShortId()
        });
      }
    }

    IEnumerator ClientConnect()
    {
      if (_settings.isClient)
      {
        yield return new WaitForSeconds(.5f);
        // emit to server to know the connection from the server-side.
        _socket.Emit(Constants.EVENT_CLIENT_CONNECT, null);
      }
    }

    void Awake()
    {
      clientId = Utility.ShortId();
    }

    void Start()
    {
      print("Start network manager!");
      _settings = Settings.instance;
      print("Initiated settings!");

      print("Create websocket instance.");
      _socket = new SocketIOWrapper(FindObjectOfType<SocketIOComponent2>());
      if (_settings.isServer)
      {
        _socket.CreateInstance();
      }
      else
      {
        if (Debug.isDebugBuild)
        {
          _socket.CreateInstance();
        }
        else
        {
#if DEVELOPMENT_BUILD
          _socket.CreateInstance();
#else
          var ip = _settings.selectedServer.server.ip;
          var port = _settings.selectedServer.server.port;
          var path = _settings.selectedServer.server.path;
          _socket.CreateInstance(ip, port, path);
#endif
        }
      }
      _socket.Connect();

      _socket.On(Constants.EVENT_CLIENT_OTHER_DISCONNECTED, OnClientOtherDisconnected);
      _socket.On(Constants.EVENT_RECEIVE_EMIT_MESSAGE, OnReceiveEmitMessage);
      _socket.On(Constants.EVENT_BROADCAST_CLONE_EVERYWHERE, OnBroadcastCloneEverywhere);
      if (_settings.isServer)
      {
        print("Connecting to socket...");
        // Let's consider this event-object-register
        _socket.On(Constants.EVENT_SERVER_REGISTER, OnServerRegister);
        _socket.On(Constants.EVENT_SERVER_CONNECTED, OnServerConnected);

        // Connect to socket
        StartCoroutine(ServerConnect());
      }
      else
      {
        print("Connecting to server...");
        _socket.On(Constants.EVENT_CLIENT_REGISTER_FINISHED, OnClientRegisterFinished);
        _socket.On(Constants.EVENT_SERVER_DISCONNECTED, OnServerDisconnected);
        _socket.On(Constants.EVENT_CLIENT_CONNECTED, OnClientConnected);
        _socket.On(Constants.EVENT_BROADCAST_SERVER_CLONE_EVERYWHERE, OnBroadcastServerCloneEverywhere);
        _socket.On("score_broadcast", OnScoreBroadcast);
        // Connect to socket
        StartCoroutine(ClientConnect());
      }
    }

    void OnBroadcastCloneEverywhere(SocketEvent evt)
    {
      var dataJson = NetCloneJSON.Deserialize(evt.data);
      if (onBroadcastCloneEverywhereJson != null)
      {
        onBroadcastCloneEverywhereJson(dataJson);
      }
    }

    void OnBroadcastServerCloneEverywhere(SocketEvent evt)
    {
      var dataJson = NetServerCloneJSON.Deserialize(evt.data);
      if (onBroadcastServerCloneEverywhereJson != null)
      {
        onBroadcastServerCloneEverywhereJson(dataJson);
      }
    }

    void OnReceiveEmitMessage(SocketEvent evt)
    {
      var dataJson = NetMessageJSON.Deserialize(evt.data);
      if (onReceiveMessageJson != null)
      {
        onReceiveMessageJson(dataJson);
      }
    }

    void OnConnected(SocketEvent evt)
    {
      if (_settings.isServer)
      {
        print("Server has connected to websocket.");
        if (onServerConnected != null)
        {
          onServerConnected();
        }
        return;
      }
      print("Client has connected to websocket.");
      if (onClientConnected != null)
      {
        onClientConnected();
      }
    }

    void OnServerConnected(SocketEvent evt)
    {
      if (_settings.isServer)
      {
        print("Server has connected to socket.");
        if (onServerConnected != null)
        {
          onServerConnected();
        }
      }
    }

    void OnClientConnected(SocketEvent evt)
    {
      if (_settings.isClient)
      {
        print("Client has connected to socket.");
        if (onClientConnected != null)
        {
          onClientConnected();
        }
      }
    }

    void OnClientRegisterFinished(SocketEvent evt)
    {
      Debug.Log("Registered completely!");
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      if (onClientRegisterFinished != null)
      {
        onClientRegisterFinished(dataJSON);
      }
    }

    void OnServerDisconnected(SocketEvent evt)
    {
      Debug.Log("Disconnected from server.");
      if (onServerDisconnected != null)
      {
        onServerDisconnected();
      }
    }

    void OnObjectRegister(SocketEvent evt)
    {
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      Debug.Log("Register object");
      Debug.Log($"Registered object {{id: {dataJSON.id}, name: {dataJSON.netName}}}");
      if (onObjectRegister != null)
      {
        onObjectRegister(dataJSON);
      }
    }

    void OnServerRegister(SocketEvent evt)
    {
      var dataJSON = NetRegisterJSON.Deserialize(evt.data);
      if (onServerRegister != null)
      {
        onServerRegister(dataJSON);
      }
    }

    void OnObjectRegistered(SocketEvent evt)
    {
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      Debug.Log("Object has registered");
      Debug.Log($"Registered object {{id: {dataJSON.id}, name: {dataJSON.netName}}}");
      if (onObjectRegistered != null)
      {
        onObjectRegistered(dataJSON);
      }
    }

    void OnOtherObjectRegistered(SocketEvent evt)
    {
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      Debug.Log($"Other object {{id: {dataJSON.id}, name: {dataJSON.netName}}} has registered");
      if (onOtherObjectRegistered != null)
      {
        onOtherObjectRegistered(dataJSON);
      }
    }

    void OnClientOtherDisconnected(SocketEvent evt)
    {
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      if (onOtherDisconnected != null)
      {
        onOtherDisconnected(dataJSON);
      }
    }

    void OnScoreBroadcast(SocketEvent evt)
    {
      var dataJson = JsonUtility.FromJson<ScoreJson>(evt.data.ToString());
      if (onScoreBroadcast != null)
      {
        onScoreBroadcast.Invoke(dataJson);
      }
    }
  }
}