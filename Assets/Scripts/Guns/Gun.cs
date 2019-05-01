using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
	public GunHandType gunHandType;
	public float knockbackIndex;
	[Header ("Shells")]
	public Shell shellPrefab;
	public Transform shellEjection;
	public abstract void HoldTrigger ();
	public abstract void ReleaseTrigger ();
	public System.Action OnProjectileLaunched;

	public void EjectShell ()
	{
		// eject shells.
		Instantiate (shellPrefab, shellEjection.position, shellEjection.rotation);
	}

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
