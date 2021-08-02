using System;

namespace Net
{
  [Serializable]
  public struct NetBulletRegisterJSON
  {
    public int playerId;
    public float[] position;
    public float[] rotation;

    public NetBulletRegisterJSON (int playerId, float[] position, float[] rotation)
    {
      this.playerId = playerId;
      this.position = position;
      this.rotation = rotation;
    }
  }
}
