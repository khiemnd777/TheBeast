using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraDeathSaw : MonsterSkill
{
    public MonsterCylora host;
    public AnimationClip defaultAnim;
    public float wingSpeed;
    public float rollingTime;
    public float stopRollingTime;
    public float scaleWingsValue;
    [SerializeField]
    Transform _coreRotation;
    [SerializeField]
    AnimationCurve _stopRollingSpeedCurve;
    Player2 _player;
    bool _isStopRolling;
    Vector3 _positionAsBeforeRushForward;

    public override void Awake ()
    {
        base.Awake ();
        _player = FindObjectOfType<Player2> ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
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
        StartCoroutine (RushForward ());
        StartCoroutine (KeepRolling ());
        StartCoroutine (ScaleWingsOut ());
        yield return new WaitForSeconds (rollingTime);
        _isStopRolling = true;
        StartCoroutine (GetBackAsBeforeRushed ());
        StartCoroutine (ScaleWingsIn ());
        yield return StartCoroutine (StopRolling ());
    }

    IEnumerator RushForward ()
    {
        _positionAsBeforeRushForward = host.transform.position;
        var directionToPlayerAsNormal = _player.transform.position - _positionAsBeforeRushForward;
        directionToPlayerAsNormal.Normalize ();
        var updatedPositionAsRushedForward = _positionAsBeforeRushForward + directionToPlayerAsNormal * 1f;
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

    IEnumerator KeepRolling ()
    {
        while (!_isStopRolling)
        {
            _coreRotation.Rotate (Vector3.back * Time.deltaTime * wingSpeed);
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
