using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetHeadRotationJSON
  {
    public int id;
    public float[] rotation;

    public NetHeadRotationJSON (int id, Quaternion rotation)
    {
      this.id = id;
      this.rotation = Utility.QuaternionToAnglesArray (rotation);
    }

    public static NetHeadRotationJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetHeadRotationJSON> (data.ToString ());
    }
  }
}
