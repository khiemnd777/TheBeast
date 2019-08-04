using System.Collections;
using UnityEngine;

public abstract class MonsterSkill : MonoBehaviour
{
    public float damage;
    [System.NonSerialized]
    public MonsterSkillHandler skillHandler;
    public float timeBetweenLaunching;
    public float minDistanceExecuting;
    public float maxDistanceExecuting;
    [System.NonSerialized]
    public bool beExecuting;
    [System.NonSerialized]
    public bool beCoolingDown;
    public System.Func<IEnumerator> OnBeforeExecutingHandler;
    public System.Func<IEnumerator> OnAfterExecutingHandler;
    bool _stopExecuting;
    Coroutine _currentExecutingSkill;

    public virtual void Awake ()
    {
        skillHandler = GetComponentInParent<MonsterSkillHandler> ();
    }

    public virtual void Start ()
    {

    }

    public abstract IEnumerator OnExecuting ();

    public virtual void OnStoppingExecutingSkill ()
    {

    }

    public virtual void OnStoppedExecutingSkill ()
    {

    }

    private IEnumerator Execute ()
    {
        if (beExecuting) yield break;
        if (beCoolingDown) yield break;
        _stopExecuting = false;
        beExecuting = true;
        if (OnBeforeExecutingHandler != null)
        {
            yield return StartCoroutine (OnBeforeExecutingHandler ());
        }
        // while (true)
        // {
        //     if (timeBetweenLaunching > 0)
        //     {
        //         while (tBetweenAct <= 1f)
        //         {
        //             tBetweenAct += Time.deltaTime / timeBetweenLaunching;
        //             yield return null;
        //         }
        //     }
        //     tBetweenAct = 0f;
        //     yield return StartCoroutine (OnExecuting ());
        //     if (_stopExecuting) break;
        // }
        yield return StartCoroutine (OnExecuting ());
        if (OnAfterExecutingHandler != null)
        {
            yield return StartCoroutine (OnAfterExecutingHandler ());
        }
        beExecuting = false;
        beCoolingDown = true;
        StartCoroutine (CoolingDown ());
    }

    IEnumerator CoolingDown ()
    {
        if (timeBetweenLaunching > 0)
        {
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeBetweenLaunching;
                yield return null;
            }
        }
        beCoolingDown = false;
    }

    public void StartExecutingSkill ()
    {
        StartCoroutine ("Execute");
    }

    public void StopExecutingSkill ()
    {
        OnStoppingExecutingSkill ();
        _stopExecuting = true;
        beExecuting = false;
        StopAllCoroutines ();
        OnStoppedExecutingSkill ();
    }

    public void OnOutOfRange ()
    {
        _stopExecuting = true;
    }
}
