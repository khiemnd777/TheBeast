using System;

namespace Net
{
  [Serializable]
  public struct NetFodderJSON
  {
    public int id;
    public float[] position;

    public NetFodderJSON (int id, float[] position)
    {
      this.id = id;
      this.position = position;
    }
  }
}
