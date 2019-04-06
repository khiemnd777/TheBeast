using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	public float hp;
	public float maxHp;
	public float detectedRadius;
	protected NavMeshAgent agent;
	protected SphereCollider detectedArea;

	public virtual void Awake ()
	{
		agent = GetComponent<NavMeshAgent> ();
		detectedArea = GetComponentInChildren<SphereCollider> ();
	}

	public virtual void Start ()
	{
		detectedArea.radius = detectedRadius;
	}

	public virtual void Update ()
	{

	}
}
