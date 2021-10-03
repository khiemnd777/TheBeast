using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetServerCloneJSON
  {
    public int netId;
    public string clientId;
    public string prefabName;
    public string netName;
    public float life;
    public float maxLife;
    public float lifetime;
    public float[] position;
    public float[] rotation;
    public string other;

    public bool stored;

    public NetServerCloneJSON(int netId, string clientId, string prefabName, string netName, float life, float maxLife, float lifetime, Point point, Quaternion rotation, object other, bool stored = false)
    {
      this.netId = netId;
      this.clientId = clientId;
      this.prefabName = prefabName;
      this.netName = netName;
      this.life = life;
      this.maxLife = maxLife;
      this.lifetime = lifetime;
      this.position = new[] { point.x, point.y };
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
      this.other = other == null ? string.Empty : JsonUtility.ToJson(other);
      this.stored = stored;
    }

    public static NetServerCloneJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetServerCloneJSON>(data.ToString());
    }

    public static Point ToPoint(NetServerCloneJSON netObjectJson)
    {
      var position = netObjectJson.position;
      return new Point(position[0], position[1]);
    }

    public static Quaternion ToQuaternion(NetServerCloneJSON netObjectJson)
    {
      var rotation = netObjectJson.rotation;
      return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
    }
  }
}
