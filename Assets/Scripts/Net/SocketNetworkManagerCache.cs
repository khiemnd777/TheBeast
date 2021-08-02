using Net;
using Net.Socket;
using UnityEngine;

public class SocketNetworkManagerCache
{
  static SocketNetworkManager instance;
  static NetworkManager networkManagerInstance;
  static ISocketWrapper pSocket;

  /// <summary>
  /// Returns the instance of SocketNetworkManager.
  /// </summary>
  /// <returns></returns>
  public static SocketNetworkManager GetInstance ()
  {
    return instance ?? (instance = GameObject.FindObjectOfType<SocketNetworkManager> ());
  }

  public static NetworkManager GetNetworkManager ()
  {
    return networkManagerInstance ?? (networkManagerInstance = GameObject.FindObjectOfType<NetworkManager> ());
  }

  /// <summary>
  /// Returns the cache of ISocketWrapper.
  /// </summary>
  /// <returns></returns>
  public static ISocketWrapper GetSocket ()
  {
    return pSocket ?? (pSocket = GetInstance ().socket);
  }

  /// <summary>
  /// Returns the cache of the ISocketWrapper.
  /// </summary>
  /// <value></value>
  public static ISocketWrapper socket
  {
    get
    {
      return pSocket ?? (pSocket = GetInstance ().socket);
    }
  }
}
