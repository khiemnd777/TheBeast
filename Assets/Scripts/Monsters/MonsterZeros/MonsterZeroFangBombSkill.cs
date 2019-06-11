using System.Collections;
using UnityEngine;

public class MonsterZeroFangBombSkill : MonsterSkill
{
    public MonsterZero host;
	public MonsterZeroFangBomb fangBombPrefab;
    public AnimationClip defaultAnim;
    public AnimationClip fangBombAnim;
    public Transform projectileBomb;
    public float timeFangBombLaunching;

    public override void Awake ()
    {
        base.Awake ();
        OnBeforeExecutingHandler += OnBeforeExecuting;
        OnAfterExecutingHandler += OnAfterExecuting;
    }

    IEnumerator OnBeforeExecuting ()
    {
        host.animator.Play (fangBombAnim.name, 0, 0);
        yield break;
    }

    public override IEnumerator OnExecuting ()
    {
        host.StopMoving ();
        var fangBombIns = Instantiate<MonsterZeroFangBomb> (fangBombPrefab, projectileBomb.position, Quaternion.identity);
        fangBombIns.projectile = projectileBomb;
        // Launching...
        var tLaunching = 0f;
        while (tLaunching <= 1f)
        {
            tLaunching += Time.deltaTime / timeFangBombLaunching;
            yield return null;
        }
        fangBombIns.Launch ();
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
