using System;
using SocketIO;
using UnityEngine;

namespace Net.Socket
{
  public class SocketIOWrapper : MonoBehaviour, ISocketWrapper
  {
    SocketIOComponent2 _socket;

    public SocketIOWrapper(SocketIOComponent2 socket)
    {
      _socket = socket;
    }

    public void CreateInstance(string ip, int port, string path)
    {
      _socket.ip = ip;
      _socket.port = port;
      _socket.path = path;
      _socket.CreateInstance();
    }

    public void CreateInstance()
    {
      _socket.CreateInstance();
    }

    public void Connect()
    {
      _socket.Connect();
    }

    public void Emit(string evt)
    {
      _socket.Emit(evt);
    }

    public void Emit(string evt, object data)
    {
      _socket.Emit(evt, new JSONObject(JsonUtility.ToJson(data)));
    }

    public void Emit(string evt, string json)
    {
      _socket.Emit(evt, new JSONObject(json));
    }

    public void On(string evt, Action<SocketEvent> callback)
    {
      _socket.On(evt, (socketEvent) =>
     {
       if (callback != null)
       {
         callback.Invoke(new SocketEvent(socketEvent.name, socketEvent.data));
       }
     });
    }
  }
}
