using System;

namespace Net
{
  [Serializable]
  public struct NetSocketIdJSON
  {
    public string socketId;

    public NetSocketIdJSON (string socketId)
    {
      this.socketId = socketId;
    }
  }
}
