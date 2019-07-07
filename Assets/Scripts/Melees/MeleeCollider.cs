using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
    public float size;
    [SerializeField]
    BoxCollider _collider;

    void OnDrawGizmos ()
    {
        var center = transform.position;
        center.x += _collider.center.x;
        Gizmos.DrawWireCube (center, _collider.size);
    }
}
