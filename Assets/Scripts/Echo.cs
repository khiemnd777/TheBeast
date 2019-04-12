using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echo : MonoBehaviour
{
	[SerializeField]
	CachedEchoBeam _cachedEchoBeam;
	public int amount;
	public float speed;
	public float raycastDistance;
	public float lifetime;
	public Transform owner;

	void Awake ()
	{
		_cachedEchoBeam = FindObjectOfType<CachedEchoBeam> ();
	}

	void Start ()
	{
		Launch ();
	}
	
	public void Launch ()
	{
		_cachedEchoBeam.Use (amount, transform.position, speed, raycastDistance, lifetime, owner);
	}
}
