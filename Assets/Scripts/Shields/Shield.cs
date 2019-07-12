using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
	public float hitback;
	protected ShieldHolder holder;
	[System.NonSerialized]
	public bool anyAction;
	public RuntimeAnimatorController meleeAnimatorController;

	public virtual IEnumerator HoldTrigger ()
	{
		yield break;
	}

    public virtual IEnumerator ReleaseTrigger ()
	{
		yield break;
	}

	public virtual void TakeShieldUpAsCover ()
	{

	}

	public virtual void TakeShieldDown ()
	{
		Destroy (gameObject);
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

	public virtual Vector3 GetDirection ()
	{
		var direction = Utilities.GetDirection (transform, Vector3.back);
		return direction * holder.transform.localScale.z;
	}

	public Vector3 GetNormalDirection ()
	{
		var direction = GetDirection ();
		direction.Normalize ();
		return direction;
	}
}
