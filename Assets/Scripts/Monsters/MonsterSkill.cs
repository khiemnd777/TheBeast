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
    protected bool isInRange;
    bool _stopExecuting;
    Coroutine _currentExecutingSkill;

    public virtual void Awake ()
    {
        skillHandler = GetComponentInParent<MonsterSkillHandler> ();
    }

    public virtual void Start ()
    {

    }

    public virtual void Update ()
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
        yield return StartCoroutine (Execute (this));
        beCoolingDown = true;
        StartCoroutine (CoolingDown ());
    }

    protected IEnumerator Execute (MonsterSkill skill)
    {
        skill.beExecuting = true;
        if (skill.OnBeforeExecutingHandler != null)
        {
            yield return StartCoroutine (skill.OnBeforeExecutingHandler ());
        }
        yield return StartCoroutine (skill.OnExecuting ());
        if (skill.OnAfterExecutingHandler != null)
        {
            yield return StartCoroutine (skill.OnAfterExecutingHandler ());
        }
        skill.beExecuting = false;
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
        isInRange = true;
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

    public virtual void OnOutOfRange ()
    {
        isInRange = false;
        _stopExecuting = true;
    }
}
