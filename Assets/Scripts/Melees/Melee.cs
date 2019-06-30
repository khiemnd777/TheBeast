using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
	public float hitback;
	protected MeleeHolder holder;
	[System.NonSerialized]
	public bool anyAction;
	public RuntimeAnimatorController meleeAnimatorController;

	public virtual void HoldTrigger ()
	{

	}

	public virtual void TakeUpArm (MeleeHolder holder, Hand hand, Animator handAnimator, Player2 player)
	{

	}

	public virtual void KeepInCover ()
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
