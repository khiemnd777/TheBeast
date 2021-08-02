using UnityEngine;

[System.Serializable]
public struct NetRotationJSON
{
  public int id;
  public float[] rotation;

  public NetRotationJSON (int id, Quaternion rotation)
  {
    this.id = id;
    var angles = rotation.eulerAngles;
    this.rotation = new [] { angles.x, angles.y, angles.z };
  }

  public static NetRotationJSON Deserialize (string json)
  {
    return JsonUtility.FromJson<NetRotationJSON> (json);
  }

  public static NetRotationJSON Deserialize (object data)
  {
    return JsonUtility.FromJson<NetRotationJSON> (data.ToString ());
  }

  public static Quaternion ToQuaternion (NetRotationJSON netRotationJSON)
  {
    var rotation = netRotationJSON.rotation;
    return Quaternion.Euler (rotation[0], rotation[1], rotation[2]);
  }
}
