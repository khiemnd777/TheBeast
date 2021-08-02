using UnityEngine;

[System.Serializable]
public struct NetPositionJSON
{
  public int id;
  public float[] position;

  public NetPositionJSON (int id, Point point)
  {
    this.id = id;
    this.position = new [] { point.x, point.y };
  }

  public static NetPositionJSON Deserialize (string json)
  {
    return JsonUtility.FromJson<NetPositionJSON> (json);
  }

  public static NetPositionJSON Deserialize (object data)
  {
    return JsonUtility.FromJson<NetPositionJSON> (data.ToString ());
  }

  public static Point ToPoint (NetPositionJSON netPositionJSON)
  {
    var position = netPositionJSON.position;
    return new Point (position[0], position[1]);
  }
}
