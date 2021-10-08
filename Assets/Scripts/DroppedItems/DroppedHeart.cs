using Net;
using UnityEngine;

public class DroppedHeart : DroppedItem
{
  [Range(0, 100)]
  public float percent;

  protected override void Awake()
  {
    base.Awake();
    // this.SetNetIdAtServerSide(1);
    // this.SetNetIdAtClientSide(1);
    // NetObjectList.instance.Store(this);
  }

  public override bool OnPickUp(Player player)
  {
    if (isServer)
    {
      if (player.lifeEnd) return false;
      Debug.Log($"Player {player.netName} has picked up the heart {percent}");
      var hp = player.life + player.maxLife * (percent / 100f);
      player.AddHp(hp);
      return true;
    }
    return false;
  }
}