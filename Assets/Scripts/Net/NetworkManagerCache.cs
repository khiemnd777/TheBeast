using Net;
using Net.Socket;
using UnityEngine;

public class NetworkManagerCache
{

  static NetworkManager networkManagerInstance;
  public static NetworkManager networkManager
  {
    get
    {
      return networkManagerInstance ?? (networkManagerInstance = GameObject.FindObjectOfType<NetworkManager>());
    }
  }

  static ISocketWrapper pSocket;
  public static ISocketWrapper socket
  {
    get
    {
      return pSocket ?? (pSocket = networkManager.socket);
    }
  }
}
