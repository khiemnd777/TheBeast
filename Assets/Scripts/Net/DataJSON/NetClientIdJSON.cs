using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetClientIdJSON
  {
    public string clientId;

    public NetClientIdJSON (string clientId)
    {
      this.clientId = clientId;
    }

    public static NetClientIdJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetClientIdJSON>(data.ToString());
    }
  }
}
