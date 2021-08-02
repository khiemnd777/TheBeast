using System;

namespace Net
{
  [Serializable]
  public struct NetHpPickerJSON
  {
    public int id;
    public float[] targetPosition;
    public float[] position;
    public float scale;
    public float hp;

    public NetHpPickerJSON (int id, float[] targetPosition, float[] position, float scale, float hp)
    {
      this.id = id;
      this.targetPosition = targetPosition;
      this.position = position;
      this.scale = scale;
      this.hp = hp;
    }
  }
}
