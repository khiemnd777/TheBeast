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
    bool _stopExecuting;

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
        _stopExecuting = false;
        beExecuting = true;
        var tBetweenAct = 1f;
        if (OnBeforeExecutingHandler != null)
        {
            yield return StartCoroutine (OnBeforeExecutingHandler ());
        }
        while (true)
        {
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
            if (_stopExecuting) break;
        }
        if (OnAfterExecutingHandler != null)
        {
            yield return StartCoroutine (OnAfterExecutingHandler ());
        }
        beExecuting = false;
    }

    public void OnOutOfRange ()
    {
        _stopExecuting = true;
    }
}
