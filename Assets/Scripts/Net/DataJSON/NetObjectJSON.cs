using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetObjectJSON
  {
    public string clientId;
    public int id;
    public string prefabName;
    public string netName;
    public float life;
    public float maxLife;
    public float[] position;
    public float[] rotation;

    public NetObjectJSON(string clientId, int id, string prefabName, string netName, float life, float maxLife, Point point, Quaternion rotation)
    {
      this.clientId = clientId;
      this.id = id;
      this.prefabName = prefabName;
      this.netName = netName;
      this.life = life;
      this.maxLife = maxLife;
      this.position = new[] { point.x, point.y };
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
    }

    public static NetObjectJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetObjectJSON>(data.ToString());
    }

    public static Point ToPoint(NetObjectJSON netObjectJson)
    {
      var position = netObjectJson.position;
      return new Point(position[0], position[1]);
    }

    public static Quaternion ToQuaternion(NetObjectJSON netObjectJson)
    {
      var rotation = netObjectJson.rotation;
      return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
    }
  }
}
