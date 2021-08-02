using System;

namespace Net
{
  [Serializable]
  public struct NetIdentityJSON
  {
    public int id;

    public NetIdentityJSON (int id)
    {
      this.id = id;
    }
  }
}
