using Net;
using Net.Socket;
using UnityEngine;

public class NetScore : MonoBehaviour
{
  public int score;

  ISocketWrapper _socket;

  void Awake()
  {
    _socket = NetworkManagerCache.socket;
  }

  public void ServerScore(int playerNetId, string clientId)
  {
      Debug.Log($"before score: {score}");
      ++score;
      Debug.Log($"after score: {score}");
      _socket.Emit("score", new ScoreJson
      {
        clientId = clientId,
        playerNetId = playerNetId,
        score = score
      });
  }

  static object scoreObj = new object();
  public void ClientScore(int score)
  {
    lock (scoreObj)
    {
      this.score = score;
    }
  }
}

public struct ScoreJson
{
  public string clientId;
  public int playerNetId;
  public int score;
}