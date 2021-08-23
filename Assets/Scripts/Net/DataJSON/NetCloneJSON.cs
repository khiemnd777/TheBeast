using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetCloneJSON
  {
    public string clientId;
    public string prefabName;
    public string netName;
    public float life;
    public float maxLife;
    public float lifetime;
    public float[] position;
    public float[] rotation;

    public NetCloneJSON(string clientId, string prefabName, string netName, float life, float maxLife, float lifetime, Point point, Quaternion rotation)
    {
      this.clientId = clientId;
      this.prefabName = prefabName;
      this.netName = netName;
      this.life = life;
      this.maxLife = maxLife;
      this.lifetime = lifetime;
      this.position = new[] { point.x, point.y };
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
    }

    public static NetCloneJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetCloneJSON>(data.ToString());
    }

    public static Point ToPoint(NetCloneJSON netObjectJson)
    {
      var position = netObjectJson.position;
      return new Point(position[0], position[1]);
    }

    public static Quaternion ToQuaternion(NetCloneJSON netObjectJson)
    {
      var rotation = netObjectJson.rotation;
      return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
    }
  }
}
