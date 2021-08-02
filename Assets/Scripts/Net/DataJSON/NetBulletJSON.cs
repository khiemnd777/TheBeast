using System;

namespace Net
{
  [Serializable]
  public struct NetBulletJSON
  {
    public int id;
    public int playerId;
    public float[] position;
    public float[] rotation;

    public NetBulletJSON (int id, int playerId, float[] position, float[] rotation)
    {
      this.id = id;
      this.playerId = playerId;
      this.position = position;
      this.rotation = rotation;
    }
  }
}
