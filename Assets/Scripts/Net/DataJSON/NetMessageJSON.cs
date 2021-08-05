using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetMessageJSON
  {
    public string cliendId;
    public int id;
    public string eventName;
    public string message;

    public NetMessageJSON(string clientId, int id, string eventName, object message)
    {
      this.cliendId = clientId;
      this.id = id;
      this.eventName = eventName;
      this.message = JsonUtility.ToJson(message);
    }

    public static NetMessageJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetMessageJSON>(data.ToString());
    }
  }
}
