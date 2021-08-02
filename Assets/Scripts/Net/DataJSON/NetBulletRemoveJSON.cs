using System;

namespace Net
{
  [Serializable]
  public struct NetBulletRemoveJSON
  {
    public int id;
    public int playerId;

    public NetBulletRemoveJSON (int id, int playerId)
    {
      this.id = id;
      this.playerId = playerId;
    }
  }
}
