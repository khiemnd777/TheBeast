using System;
using System.Collections;
using UnityEngine;

public class MonsterZeroBiteSkill : MonsterSkill
{
    public MonsterZero host;
    public AnimationClip defaultAnim;
    public AnimationClip biteAnim;

    public override void Awake ()
    {
        base.Awake ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
    }

    public override IEnumerator OnExecuting ()
    {
        host.animator.Play (biteAnim.name, 0, 0);
        yield return new WaitForSeconds (biteAnim.length);
    }

    IEnumerator OnBeforeExecuting()
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
