using System;
using System.Collections;
using UnityEngine;

public class MonsterZeroBiteSkill : MonsterSkill
{
    public MonsterZero host;
    public AnimationClip defaultAnim;
    public AnimationClip biteAnim;
    public MonsterZeroFang leftFang;
    public MonsterZeroFang rightFang;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;

    public override void Awake ()
    {
        base.Awake ();
        _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
        _cameraShake = FindObjectOfType<CameraShake> ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
        leftFang.onHit += OnFangHit;
        rightFang.onHit += OnFangHit;
    }

    void OnFangHit (MonsterZeroFang fang, Collider other)
    {
        if (!fang.weaponEntity.anyAction) return;
        if (!other) return;
        var hitPlayer = other.GetComponent<Player2> ();
        if (hitPlayer && !hitPlayer.isFendingOff)
        {
            var contactPoint = other.ClosestPointOnBounds (transform.position);
            var dir = other.transform.position - contactPoint;
            dir.Normalize ();
            hitPlayer.OnHit (damage, fang.weaponEntity.knockbackForce, dir, contactPoint);
            _slowMotionMonitor.Freeze (.05f, .05f);
            _cameraShake.Shake (.08f, 0.125f);
        }
    }

    public override IEnumerator OnExecuting ()
    {
        host.animator.Play (biteAnim.name, 0, 0);
        var p = 0f;
        while (p <= 1f)
        {
            if (p >= .45f)
            {
                leftFang.weaponEntity.anyAction = true;
                rightFang.weaponEntity.anyAction = true;
            }
            p += Time.deltaTime / biteAnim.length;
            yield return null;
        }
        leftFang.weaponEntity.anyAction = false;
        rightFang.weaponEntity.anyAction = false;
    }

    IEnumerator OnBeforeExecuting ()
    {
        host.StopMoving ();
        yield break;
    }

    IEnumerator OnAfterExecuting ()
    {
        host.animator.Play (defaultAnim.name, 0, 0);
        host.KeepMoving ();
        yield break;
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
