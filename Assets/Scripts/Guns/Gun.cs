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
	[Header ("Heat system")]
	public float heatMax;
	public float heatUpStep;
	public float coolDownStep;
	public float heat;
	public abstract void HoldTrigger ();
	public abstract void ReleaseTrigger ();
	public System.Action OnProjectileLaunched;

	public void EjectShell ()
	{
		// eject shells.
		Instantiate (shellPrefab, shellEjection.position, shellEjection.rotation);
	}

	public void HeatUp ()
	{
		heat += heatUpStep;
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
