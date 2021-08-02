using System;
using UnityEngine;

namespace Net
{
  [Serializable]
  public struct NetWeaponTriggerJSON
  {
    public int id;

    public static NetWeaponTriggerJSON Deserialize (object data)
    {
      return JsonUtility.FromJson<NetWeaponTriggerJSON> (data.ToString ());
    }
  }
}
