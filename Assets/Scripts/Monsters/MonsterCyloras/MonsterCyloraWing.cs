using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraWing : MonoBehaviour
{
    public System.Action<MonsterCyloraWing, Collider> onHit;
    [System.NonSerialized]
    public MonsterWeaponEntity weaponEntity;

    public void TurnOffCollider ()
    {
        GetComponent<BoxCollider> ().enabled = false;
    }

    public void TurnOnCollider ()
    {
        GetComponent<BoxCollider> ().enabled = true;
    }

    void Awake ()
    {
        weaponEntity = GetComponent<MonsterWeaponEntity> ();
    }

    void OnTriggerEnter (Collider other)
    {
        if (onHit != null)
        {
            onHit (this, other);
        }
    }
}
