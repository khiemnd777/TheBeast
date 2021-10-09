using UnityEngine;

public class GunRegistrar : MonoBehaviour
{
  public GunPrefabField[] fields;
}

[System.Serializable]
public struct GunPrefabField
{
  public string name;
  public NetGun prefab;
}