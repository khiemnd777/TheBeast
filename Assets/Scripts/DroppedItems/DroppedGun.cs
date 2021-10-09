using Net;
using UnityEngine;

public class DroppedGun : DroppedItem
{
  public NetGun gunPrefab;
  public new string prefabName;
  public string gunPrefabName;

  protected override void Awake()
  {
    base.Awake();
    // this.SetNetIdAtServerSide(GetInstanceID());
    // this.SetNetIdAtClientSide(GetInstanceID());
    // NetObjectList.instance.Store(this);
  }

  public override bool OnPickUp(Player player)
  {
    if (isServer)
    {
      if (player.lifeEnd) return false;
      return true;
    }
    return false;
  }
}