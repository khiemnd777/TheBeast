using System;

namespace Net
{
  [Serializable]
  public struct NetRegisterFinishedJSON
  {
    public string clientId;
    public string playerName;
    public int id;
    public float[] position;
    public float hp;

    public NetRegisterFinishedJSON (
      string clientId,
      string playerName,
      int id,
      float[] position,
      float hp
    )
    {
      this.clientId = clientId;
      this.playerName = playerName;
      this.id = id;
      this.position = position;
      this.hp = hp;
    }
  }
}
