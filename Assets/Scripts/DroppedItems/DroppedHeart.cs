using Net;
using UnityEngine;

public class DroppedHeart : DroppedItem
{
  [Range(0, 100)]
  public float percent;

  protected override void Awake()
  {
    base.Awake();
    this.SetNetIdAtServerSide(1);
    this.SetNetIdAtClientSide(1);
    NetObjectList.instance.Store(this);
  }

  public override void PickUp(Player player)
  {
    if (isServer)
    {
      var hp = player.life + player.maxLife * (percent / 100f);
      player.AddHp(hp);
    }
    base.PickUp(player);
  }
}