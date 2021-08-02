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
	protected Player player;

	public virtual IEnumerator HoldTrigger ()
	{
		yield break;
	}

	public virtual void TakeUpArm (MeleeHolder holder, Hand hand, Animator handAnimator, Player player)
	{

	}

	public virtual void KeepInCover ()
	{
		Destroy (gameObject);
	}

	public virtual void Awake ()
	{
		player = FindObjectOfType<Player> ();
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
		var direction = Utility.GetDirection (transform, Vector3.back);
		return direction * holder.transform.localScale.z;
	}

	public Vector3 GetNormalDirection ()
	{
		var direction = GetDirection ();
		direction.Normalize ();
		return direction;
	}
}
