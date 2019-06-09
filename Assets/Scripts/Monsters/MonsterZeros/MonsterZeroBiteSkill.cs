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
        OnAfterExecutingHandler += OnAfterExecuting;
    }

    public override IEnumerator OnExecuting ()
    {
        host.StopMoving ();
        host.animator.Play (biteAnim.name, 0, 0);
        yield return new WaitForSeconds (biteAnim.length);
        host.animator.Play (defaultAnim.name, 0, 0);
    }

    IEnumerator OnAfterExecuting ()
    {
        host.animator.Play (defaultAnim.name, 0, 0);
        host.KeepMoving ();
        yield break;
    }
}
