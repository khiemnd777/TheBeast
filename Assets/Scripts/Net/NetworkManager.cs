using System;
using System.Collections;
using Net.Socket;
using SocketIO;
using UnityEngine;

namespace Net
{
  public class NetworkManager : MonoBehaviour
  {
    public Guid clientId { get; private set; }
    public event Action onConnected;
    public event Action onServerConnected;
    public event Action<NetObjectJSON> onObjectRegister;
    public event Action<NetObjectJSON> onObjectRegistered;
    public event Action<NetObjectJSON> onOtherObjectRegistered;
    public event Action<NetObjectJSON> onOtherDisconnected;
    public event Action<NetObjectJSON> onClientRegisterFinished;
    public event Action<NetRegisterJSON> onServerRegister;

    Settings _settings;
    ISocketWrapper _socket;
    public ISocketWrapper socket
    {
      get
      {
        return _socket ?? (_socket = new SocketIOWrapper(FindObjectOfType<SocketIOComponent2>()));
      }
    }

    public IEnumerator Connect()
    {
      yield return new WaitForSeconds(.5f);
      // emit to server to know the connection.
      _socket.Emit(Constants.EVENT_CONNECT, new NetConnectionJSON(_settings.isServer));
    }

    void Awake()
    {
      clientId = Guid.NewGuid();
    }

    void Start()
    {
      _settings = Settings.instance;
      _socket.On(Constants.EVENT_CLIENT_CONNECTED, OnConnected);
      _socket.On(Constants.EVENT_OBJECT_REGISTERED, OnObjectRegistered);
      _socket.On(Constants.EVENT_OTHER_OBJECT_REGISTERED, OnOtherObjectRegistered);
      _socket.On(Constants.EVENT_CLIENT_OTHER_DISCONNECTED, OnClientOtherDisconnected);
      if (_settings.isServer)
      {
        print("Connecting to socket...");
        _socket.On(Constants.EVENT_OBJECT_REGISTER, OnObjectRegister);
        _socket.On(Constants.EVENT_SERVER_REGISTER, OnServerRegister);
      }
      else
      {
        print("Connecting to server...");
        _socket.On(Constants.EVENT_CLIENT_REGISTER_FINISHED, OnClientRegisterFinished);
      }
      StartCoroutine(Connect());
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
      if (onConnected != null)
      {
        onConnected();
      }
    }

    void OnClientRegisterFinished(SocketEvent evt)
    {
      var dataJSON = NetObjectJSON.Deserialize(evt.data);
      if (onClientRegisterFinished != null)
      {
        onClientRegisterFinished(dataJSON);
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
  }
}