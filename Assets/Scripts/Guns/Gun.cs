using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
	public GunHandType gunHandType;
	public float knockbackIndex;
	[Header ("Shells")]
	public Shell shellPrefab;
	public Transform shellEjection;

	IDictionary<string, bool> _lockControlList = new Dictionary<string, bool> ();

	public abstract void HoldTrigger ();
	public abstract void ReleaseTrigger ();
	public System.Action OnProjectileLaunched;

	public void EjectShell ()
	{
		// eject shells.
		Instantiate (shellPrefab, shellEjection.position, shellEjection.rotation);
	}

	public void RegisterLock (string name)
	{
		if (_lockControlList.ContainsKey (name)) return;
		_lockControlList.Add (name, false);
	}

	public void Lock (string name)
	{
		if (!_lockControlList.ContainsKey (name)) return;
		_lockControlList[name] = true;
	}

	public void Unlock (string name)
	{
		if (!_lockControlList.ContainsKey (name)) return;
		_lockControlList[name] = false;
	}

	public bool IsLocked ()
	{
		return _lockControlList.Values.Any (locked => locked);
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
