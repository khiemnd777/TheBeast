using UnityEngine;

[CreateAssetMenu(fileName = "Server", menuName = "Server/Add", order = 0)]
public class ServerSO : ScriptableObject
{
  public ServerObject[] servers;
}

