﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraWing : MonoBehaviour
{
    public System.Action<MonsterCyloraWing, Collider> onHit;
    [System.NonSerialized]
    public MonsterWeaponEntity weaponEntity;

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