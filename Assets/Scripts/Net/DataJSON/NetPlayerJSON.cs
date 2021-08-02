using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetPlayerJSON
  {
    /// <summary>
    /// The player's identity
    /// </summary>
    public int id;

    /// <summary>
    /// The player's name.
    /// </summary>
    public string name;

    /// <summary>
    /// The player's health point.
    /// </summary>
    public float hp;

    /// <summary>
    /// The player's MAX health point.
    /// </summary>
    public float maxHp;

    public static NetPlayerJSON Deserialize (string json)
    {
      return JsonUtility.FromJson<NetPlayerJSON> (json);
    }

    public static NetPlayerJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetPlayerJSON> (data.ToString ());
    }
  }
}
