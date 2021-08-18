using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetMessageJSON
  {
    public string clientId;
    public int id;
    public string prefabName;
    public string netName;
    public float life;
    public float maxLife;
    public float[] position;
    public float[] rotation;
    public string eventName;
    public string message;

    public NetMessageJSON(string clientId, int id, string prefabName, string netName, float life, float maxLife, float[] position, float[] rotation, string eventName, object message)
    {
      this.clientId = clientId;
      this.id = id;
      this.prefabName = prefabName;
      this.netName = netName;
      this.life = life;
      this.maxLife = maxLife;
      this.position = position;
      this.rotation = rotation;
      this.eventName = eventName;
      this.message = message == null ? string.Empty : JsonUtility.ToJson(message);
    }

    public static NetMessageJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetMessageJSON>(data.ToString());
    }
  }
}
