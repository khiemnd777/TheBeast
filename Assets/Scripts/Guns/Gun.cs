using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
	public GunHandType gunHandType;
	public float knockbackIndex;
	public RuntimeAnimatorController gunAnimatorController;
	public AnimationClip gunHandTypeAnimation;
	[Header ("Shells")]
	public Shell shellPrefab;
	public Transform shellEjection;
	
	protected Player2 player;
	IDictionary<string, bool> _lockControlList = new Dictionary<string, bool> ();

	public abstract void HoldTrigger ();
	public abstract void ReleaseTrigger ();
	public System.Action OnProjectileLaunched;

	public virtual void TakeUpArm ()
	{
		player.animator.runtimeAnimatorController = gunAnimatorController;
		player.animator.Play (gunHandTypeAnimation.name, 0);
	}

	public virtual void KeepInCover ()
	{
		Destroy (gameObject);
	}

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
		player = FindObjectOfType<Player2> ();
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
