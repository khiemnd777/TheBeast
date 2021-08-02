using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetEyeMoveJSON
  {
    public int id;
    public int side;
    public float[] position;

    /// <summary>
    /// update doc later.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static NetEyeMoveJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetEyeMoveJSON> (data.ToString ());
    }

    /// <summary>
    /// update doc later.
    /// </summary>
    /// <param name="netEyeMoveJSON"></param>
    /// <returns></returns>
    public static Vector3 ToVector3 (NetEyeMoveJSON netEyeMoveJSON)
    {
      var position = netEyeMoveJSON.position;
      return new Vector3 (position[0], position[1], position[2]);
    }
  }
}
