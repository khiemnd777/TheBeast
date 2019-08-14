using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCyloraFeatherCannon : MonsterSkill
{
    public MonsterCylora host;
    public Transform coreRotation;
    public Transform projectile;
    public float speed;
    public int minAmount;
    public int maxAmount;
    public float minExecutingTime;
    public float maxExecutingTime;
    public AnimationClip normalStateClip;
    public MonsterCyloraFeather featherPrefab;
    Player2 _player;
    bool _stop;

    public override void Awake ()
    {
        _player = FindObjectOfType<Player2> ();
    }

    public override IEnumerator OnExecuting ()
    {
        _stop = false;
        host.animator.Play (normalStateClip.name, 0);
        StartCoroutine (LaunchTheFeather ());
        var executingTime = Random.Range (minExecutingTime, maxExecutingTime);
        yield return new WaitForSeconds (executingTime);
        _stop = true;
        yield return null;
    }

    void InstantiateTheFeather (MonsterCyloraFeather featherPrefab, Transform projectile)
    {
        var wingDir = (projectile.transform.position - host.transform.position);
        var wingNormal = Vector3.Normalize (wingDir);
        var featherRot = Utilities.RotateByNormal (wingNormal, Vector3.up);
        var feather = Instantiate<MonsterCyloraFeather> (featherPrefab, projectile.transform.position, Quaternion.identity);
        feather.damage = damage;
        feather.speed = speed;
        feather.transform.rotation = featherRot;
        Destroy (feather.gameObject, 3f);
    }

    IEnumerator LaunchTheFeather ()
    {
        while (!_stop)
        {
            var count = Random.Range (minAmount, maxAmount);
            while (count-- >= 0)
            {
                RotateToPlayer2 ();
                InstantiateTheFeather (featherPrefab, projectile);
                yield return new WaitForSeconds (.09f);
            }
            yield return new WaitForSeconds (1.1f);
        }
    }

    IEnumerator RotateToPlayer ()
    {
        while (!_stop)
        {
            var direction = host.transform.position - _player.transform.position;
            var normal = Vector3.Normalize (direction);
            var rot = Utilities.RotateByNormal (normal, Vector3.up, 180f);
            var newEuler = rot.eulerAngles + Vector3.up * Random.Range (-7f, 7f);
            coreRotation.rotation = Quaternion.Euler (newEuler);
            yield return null;
        }
    }

    void RotateToPlayer2 ()
    {
        var direction = host.transform.position - _player.transform.position;
        var normal = Vector3.Normalize (direction);
        var rot = Utilities.RotateByNormal (normal, Vector3.up, 180f);
        var newEuler = rot.eulerAngles + Vector3.up * Random.Range (-10f, 10f);
        coreRotation.rotation = Quaternion.Euler (newEuler);
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuting);
        Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuting);
    }
}
