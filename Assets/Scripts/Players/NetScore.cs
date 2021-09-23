using Net;
using Net.Socket;
using UnityEngine;

public class NetScore : MonoBehaviour
{
  [System.NonSerialized]
  public int score = 0;

  ISocketWrapper _socket;

  void Awake()
  {
    _socket = NetworkManagerCache.socket;
  }

  public void Score(int playerNetId)
  {
    var fromPlayer = (Player)NetObjectList.instance.Find(playerNetId);
    if (fromPlayer)
    {
      ++score;
      _socket.Emit("score", new ScoreJson
      {
        clientId = fromPlayer.clientId,
        playerNetId = playerNetId,
        score = score
      });
    }
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