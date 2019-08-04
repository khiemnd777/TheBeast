using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraFeather : MonoBehaviour
{
    void Update ()
    {
        var d = Vector3.right;
        transform.Translate (d * Time.deltaTime * 10f);
    }

    void OnTriggerEnter (Collider other)
    {

    }
}
