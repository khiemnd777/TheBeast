using System;

namespace Net
{
  [Serializable]
  public struct NetFodderLoadingJSON
  {
    public string socketId;
    public int id;
    public float[] position;

    public NetFodderLoadingJSON (string socketId, int id, float[] position)
    {
      this.socketId = socketId;
      this.id = id;
      this.position = position;
    }
  }
}
