using System;

namespace Net
{
  [Serializable]
  public struct NetLoadingPlayerJSON
  {
    public string socketId;
    public string playerName;
    public int id;
    public float[] position;
    public float hp;
    public float maxHp;

    public NetLoadingPlayerJSON (
      string socketId,
      string playerName,
      int id,
      float[] position,
      float hp,
      float maxHp
    )
    {
      this.socketId = socketId;
      this.playerName = playerName;
      this.id = id;
      this.position = position;
      this.hp = hp;
      this.maxHp = maxHp;
    }
  }
}
