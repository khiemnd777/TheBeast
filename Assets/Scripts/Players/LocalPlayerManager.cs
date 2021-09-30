using Net;
using UnityEngine;

public class LocalPlayerManager : MonoBehaviour
{
  public event System.Action<Player> onPlayerSetup;

  [System.NonSerialized]
  public Player player;

  public void SetLocalPlayer(Player player)
  {
    this.player = player;
    if (onPlayerSetup != null)
    {
      onPlayerSetup.Invoke(player);
    }
  }

  public void SetLocalPlayer(NetIdentity netId)
  {
    var player = netId.GetComponent<Player>();
    if (player)
    {
      SetLocalPlayer(player);
    }
  }
}
