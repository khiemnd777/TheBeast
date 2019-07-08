using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaSlashEffect : MonoBehaviour
{
    public float hitback;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;
    Player2 _player;

    void Awake ()
    {
        _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
        _cameraShake = FindObjectOfType<CameraShake> ();
        _player = FindObjectOfType<Player2> ();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other)
        {
            var hitMonster = other.GetComponent<Monster> ();
            if (hitMonster)
            {
                var contactPoint = other.ClosestPointOnBounds (transform.position);
                // var dir = contactPoint - _player.transform.position;
                var dir = other.transform.position - _player.transform.position;
                dir.Normalize ();
                // dir = dir * holder.transform.localScale.z;
                hitMonster.OnHit (transform, hitback, dir, contactPoint);
                _slowMotionMonitor.Freeze (.45f, .2f);
                _cameraShake.Shake (.125f, .125f);
                return;
            }
            var reversedObject = other.GetComponent<ReversedObject> ();
            if (reversedObject)
            {
                reversedObject.reversed = true;
                reversedObject.speed *= 1.25f;
                _slowMotionMonitor.Freeze (.0625f, .2f);
                return;
            }
            var monsterWeaponEntity = other.GetComponent<MonsterWeaponEntity> ();
            if (monsterWeaponEntity && monsterWeaponEntity.anyAction)
            {
                var contactPoint = other.ClosestPointOnBounds (transform.position);
                var dir = _player.transform.position - contactPoint;
                dir.Normalize ();
                _player.OnFendingOff (monsterWeaponEntity.knockbackForce, dir, contactPoint);
                _slowMotionMonitor.Freeze (.08f, .08f);
                return;
            }
        }
    }
}
