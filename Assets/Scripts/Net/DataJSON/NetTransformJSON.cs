using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetTransformJSON
  {
    public float[] position;
    public float[] rotation;

    public NetTransformJSON(Point point, Quaternion rotation)
    {
      this.position = new[] { point.x, point.y };
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
    }

    public NetTransformJSON(Vector3 point, Quaternion rotation)
    {
      this.position = new[] { point.x, point.z };
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
    }

    public static NetTransformJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetTransformJSON>(data.ToString());
    }

    public static Point ToPoint(NetTransformJSON netTransformJson)
    {
      var position = netTransformJson.position;
      return new Point(position[0], position[1]);
    }

    public static Quaternion ToQuaternion(NetTransformJSON netTransformJson)
    {
      var rotation = netTransformJson.rotation;
      return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
    }
  }
}
