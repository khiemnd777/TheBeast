using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	public float hp;
	public float maxHp;
	public float detectedRadius;
	public float refreshRate = 1f;
	public float speed;
	public float timeDetectTarget = 1f;
	[System.NonSerialized]
	public Transform target;
	protected NavMeshAgent agent;
	protected SphereCollider detectedArea;
	float _tdt;
	Vector3 _lastTargetPos;

	public virtual void Awake ()
	{
		agent = GetComponent<NavMeshAgent> ();
		detectedArea = GetComponentInChildren<SphereCollider> ();
	}

	public virtual void Start ()
	{
		detectedArea.radius = detectedRadius;
		agent.speed = speed;
		StartCoroutine (GotoTarget ());
	}

	public virtual void Update ()
	{
		DetectTarget ();
	}

	public virtual void DetectTarget ()
	{
		// if (target == null || target is Object && !target.Equals (null))
		// {
		// 	_tdt += Time.deltaTime / timeDetectTarget;
		// 	while (_tdt >= 1f)
		// 	{
		// 		target = null;
		// 	}
		// }
		// else
		// {
		// 	_tdt = 0f;
		// }
	}

	protected IEnumerator GotoTarget ()
	{
		while (true)
		{
			if (target != null && target is Object && !target.Equals (null))
			{
				var targetPos = new Vector3 (target.position.x, 0f, target.position.z);
				agent.SetDestination (targetPos);
				_lastTargetPos = targetPos;
				yield return new WaitForSeconds (refreshRate);
			}
			yield return null;
		}
	}
}
