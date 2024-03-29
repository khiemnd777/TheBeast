﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraDeathSaw : MonsterSkill
{
    public MonsterCylora host;
    public AnimationClip defaultAnim;
    public Animator headAnimator;
    public AnimationClip openFacesAnim;
    public AnimationClip closeFacesAnim;
    public float wingSpeed;
    public float rollingTime;
    public float stopRollingTime;
    public float scaleWingsValue;
    [SerializeField]
    Transform _coreRotation;
    [SerializeField]
    AnimationCurve _stopRollingSpeedCurve;
    [SerializeField]
    MonsterCyloraWing[] _wings;
    Player2 _player;
    SlowMotionMonitor _slowMotionMonitor;
    CameraShake _cameraShake;
    bool _isStopRolling;
    Vector3 _positionAsBeforeRushForward;

    public override void Awake ()
    {
        base.Awake ();
        _player = FindObjectOfType<Player2> ();
        _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
        _cameraShake = FindObjectOfType<CameraShake> ();
        foreach (var wing in _wings)
        {
            wing.onHit += OnWingHit;
        }
    }

    void OnWingHit (MonsterCyloraWing wing, Collider other)
    {
		if (!beExecuting) return;
        if (!wing.weaponEntity.anyAction) return;
        if (!other) return;
        var hitPlayer = other.GetComponent<Player2> ();
        if (hitPlayer && !hitPlayer.isFendingOff)
        {
            var contactPoint = other.ClosestPointOnBounds (transform.position);
            var dir = other.transform.position - contactPoint;
            dir.Normalize ();
            hitPlayer.OnHit (damage, wing.weaponEntity.knockbackForce, dir, contactPoint);
            _slowMotionMonitor.Freeze (.025f, .025f);
            _cameraShake.Shake (.06f, 0.125f);
        }
    }

    void WingsInAction (bool anyAction)
    {
        foreach (var wing in _wings)
        {
            wing.weaponEntity.anyAction = anyAction;
        }
    }

    IEnumerator OnBeforeExecuting ()
    {
        host.StopMoving ();
        host.StopLeadingToTarget ();
        host.StopRotatingToTarget ();
        host.animator.enabled = false;
        yield break;
    }

    IEnumerator OnAfterExecuting ()
    {
        host.KeepMoving ();
        host.KeepLeadingToTarget ();
        host.KeepRotatingToTarget ();
        host.animator.enabled = true;
        host.animator.Play (defaultAnim.name, 0, 0);
        yield break;
    }

    public override IEnumerator OnExecuting ()
    {
        _isStopRolling = false;
        headAnimator.Play (openFacesAnim.name, 0, .75f);
        StartCoroutine (RushForward ());
        StartCoroutine (KeepRolling ());
        StartCoroutine (ScaleWingsOut ());
        WingsInAction (true);
        yield return new WaitForSeconds (rollingTime);
        _isStopRolling = true;
        headAnimator.Play (closeFacesAnim.name, 0, .75f);
        // StartCoroutine (GetBackAsBeforeRushed ());
        StartCoroutine (ScaleWingsIn ());
        yield return StartCoroutine (StopRolling ());
        WingsInAction (false);

    }

    IEnumerator RushForward ()
    {
        _positionAsBeforeRushForward = host.transform.position;
        var directionToPlayerAsNormal = _player.transform.position - _positionAsBeforeRushForward;
        directionToPlayerAsNormal.Normalize ();
        var updatedPositionAsRushedForward = _positionAsBeforeRushForward + directionToPlayerAsNormal * 1.25f;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / .2f;
            var movingLerp = Vector3.Lerp (_positionAsBeforeRushForward, updatedPositionAsRushedForward, t);
            host.transform.position = movingLerp;
            yield return null;
        }
    }

    IEnumerator GetBackAsBeforeRushed ()
    {
        var asCurrentPosition = host.transform.position;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / .2f;
            var movingLerp = Vector3.Lerp (asCurrentPosition, _positionAsBeforeRushForward, t);
            host.transform.position = movingLerp;
            yield return null;
        }
    }

    IEnumerator ScaleWingsOut ()
    {
        var t = 0f;
        var scaleWingsRate = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / .2f;
            scaleWingsRate = Mathf.Lerp (1f, scaleWingsValue, t);
            _coreRotation.localScale = Vector3.one * scaleWingsRate;
            yield return null;
        }
    }

    IEnumerator ScaleWingsIn ()
    {
        var t = 0f;
        var scaleWingsRate = scaleWingsValue;
        while (t <= 1f)
        {
            t += Time.deltaTime / .2f;
            scaleWingsRate = Mathf.Lerp (scaleWingsValue, 1f, t);
            _coreRotation.localScale = Vector3.one * scaleWingsRate;
            yield return null;
        }
    }

    IEnumerator KeepRolling ()
    {
        while (!_isStopRolling)
        {
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed);
            yield return null;
        }
    }

    IEnumerator StopRolling ()
    {
        var speedRate = 0f;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / stopRollingTime;
            speedRate = Mathf.Lerp (1f, 0f, _stopRollingSpeedCurve.Evaluate (t));
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed * speedRate);
            yield return null;
        }
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
