using UnityEngine;

namespace Net
{
  [System.Serializable]
  public struct NetLoadedPlayerJSON
  {
    /// <summary>
    /// player JSON
    /// </summary>
    public NetPlayerJSON player;
    
    /// <summary>
    /// total
    /// </summary>
    public int total;

    public static NetLoadedPlayerJSON Deserialize (string json)
    {
      return JsonUtility.FromJson<NetLoadedPlayerJSON> (json);
    }

    public static NetLoadedPlayerJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetLoadedPlayerJSON> (data.ToString ());
    }
  }
}
