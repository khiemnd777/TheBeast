using System.Collections;
using UnityEngine;

public abstract class MonsterSkill : MonoBehaviour
{
    [System.NonSerialized]
    public MonsterSkillHandler skillHandler;
    public float timeBetweenLaunching;
    public float minDistanceExecuting;
    public float maxDistanceExecuting;
    [System.NonSerialized]
    public bool beExecuting;
    public System.Func<IEnumerator> OnBeforeExecutingHandler;
    public System.Func<IEnumerator> OnAfterExecutingHandler;

    public virtual void Awake ()
    {
        skillHandler = GetComponentInParent<MonsterSkillHandler> ();
    }

    public virtual void Start ()
    {

    }

    public abstract IEnumerator OnExecuting ();

    public virtual IEnumerator Execute ()
    {
        if (beExecuting) yield break;
        beExecuting = true;
        var tBetweenAct = 1f;
        while (true)
        {
            if (OnBeforeExecutingHandler != null)
            {
                yield return StartCoroutine (OnBeforeExecutingHandler ());
            }
            if (timeBetweenLaunching > 0)
            {
                while (tBetweenAct <= 1f)
                {
                    tBetweenAct += Time.deltaTime / timeBetweenLaunching;
                    yield return null;
                }
            }
            tBetweenAct = 0f;
            yield return StartCoroutine (OnExecuting ());
            if (!skillHandler.accessExecutingSkill) break;
        }
        if (OnAfterExecutingHandler != null)
        {
            yield return StartCoroutine (OnAfterExecutingHandler ());
        }
        beExecuting = false;
    }
}
