using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
	public GunHandType gunHandType;
	public float knockbackIndex;
	public abstract IEnumerator HoldTrigger ();
	public System.Action OnProjectileLaunched;

	public virtual void Awake ()
	{

	}

	public virtual void Start ()
	{

	}

	public virtual void Update ()
	{

	}

	public virtual void FixedUpdate ()
	{

	}
}
