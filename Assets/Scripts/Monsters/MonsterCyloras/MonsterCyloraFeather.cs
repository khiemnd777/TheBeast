using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraFeather : MonoBehaviour
{
    public float damage;
    public float knockbackForce;
    public float speed = 10f;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;

    void Awake ()
    {
        _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
        _cameraShake = FindObjectOfType<CameraShake> ();
    }

    void Update ()
    {
        var d = Vector3.right;
        transform.Translate (d * Time.deltaTime * speed);
    }

    void OnTriggerEnter (Collider other)
    {
        var hitPlayer = other.GetComponent<Player2> ();
        if (hitPlayer && !hitPlayer.isFendingOff)
        {
            var contactPoint = other.ClosestPointOnBounds (transform.position);
            var dir = other.transform.position - contactPoint;
            dir.Normalize ();
            hitPlayer.OnHit (damage, knockbackForce, dir, contactPoint);
            // _slowMotionMonitor.Freeze (.1f, .1f);
            _cameraShake.Shake (.2f, 0.35f);
        }
    }
}
