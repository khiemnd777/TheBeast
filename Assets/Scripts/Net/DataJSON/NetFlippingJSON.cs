using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetFlippingJSON
  {
    public int id;
    public float sign;
    public int direction;

    /// <summary>
    /// update doc later.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static NetFlippingJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetFlippingJSON> (data.ToString ());
    }
  }
}
