using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetRegisterJSON
  {
    public string clientId;
    public string prefabName;
    public string netName;

    public NetRegisterJSON(string clientId, string prefabName, string netName)
    {
      this.clientId = clientId;
      this.prefabName = prefabName;
      this.netName = netName;
    }

    public static NetRegisterJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetRegisterJSON>(data.ToString());
    }
  }
}
