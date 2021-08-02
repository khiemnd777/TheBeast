using System;

namespace Net
{
  [Serializable]
  public struct NetConnectionJSON
  {
    public bool isServer;

    public NetConnectionJSON (bool isServer)
    {
      this.isServer = isServer;
    }
  }
}
