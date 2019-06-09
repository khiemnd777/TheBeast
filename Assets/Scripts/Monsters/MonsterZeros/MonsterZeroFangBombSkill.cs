using System.Collections;
using UnityEngine;

public class MonsterZeroFangBombSkill : MonsterSkill
{
    public MonsterZero host;
    public AnimationClip defaultAnim;

    public override void Awake ()
    {
        base.Awake ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
    }
    
    IEnumerator OnBeforeExecuting ()
    {
        yield break;
    }

    public override IEnumerator OnExecuting ()
    {
        yield break;
    }

    IEnumerator OnAfterExecuting ()
    {
        yield break;
    }

}
