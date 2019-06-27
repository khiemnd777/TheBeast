using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraWing : MonoBehaviour
{
    BoxCollider _collider;
    public System.Action<MonsterCyloraWing, Collider> onHit;

    void Awake ()
    {
        _collider = GetComponent<BoxCollider> ();
    }

    void OnTriggerEnter (Collider other)
    {
        if (onHit != null)
        {
            onHit (this, other);
        }
    }
}
