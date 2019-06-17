using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
	public RuntimeAnimatorController meleeAnimatorController;

	public virtual void HoldTrigger ()
	{

	}

	public virtual void TakeUpArm (Hand hand, Animator handAnimator, Player2 player)
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
}
