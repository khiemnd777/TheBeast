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
	public float initSpeed;
	public float timeDetectTarget = 1f;
	[System.NonSerialized]
	public Transform target;
	protected NavMeshAgent agent;
	protected DetectedListenedSoundArea detectedArea;
	float _tdt;
	Vector3 _lastTargetPos;

	public virtual void Awake ()
	{
		agent = GetComponent<NavMeshAgent> ();
		detectedArea = GetComponentInChildren<DetectedListenedSoundArea> ();
		detectedArea.radius = detectedRadius;
		agent.speed = initSpeed;
	}

	public virtual void Start ()
	{
		StartCoroutine (LeadtoTarget ());
	}

	public virtual void Update ()
	{
		
	}

	protected IEnumerator LeadtoTarget ()
	{
		while (true)
		{
			if (detectedArea.detectedPosition != Vector3.zero)
			{
				_tdt += Time.deltaTime / refreshRate;
				if (_tdt >= 1f)
				{
					agent.SetDestination (detectedArea.detectedPosition);
					_tdt = 0f;
				}
			}
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, detectedRadius);
	}
}
