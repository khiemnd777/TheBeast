using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetArmRotationJSON
  {
    public int id;
    public float[] rotation;

    public NetArmRotationJSON(int id, Quaternion rotation)
    {
      this.id = id;
      this.rotation = Utility.QuaternionToAnglesArray(rotation);
    }

    public static NetArmRotationJSON Deserialize(object data)
    {
      return JsonUtility.FromJson<NetArmRotationJSON>(data.ToString());
    }
  }
}
