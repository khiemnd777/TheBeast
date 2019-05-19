using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
	public virtual void OnHit (Transform hitBy, float hitback, RaycastHit raycastHit)
	{

	}

	public virtual void OnHit (Transform hitBy, float hitback)
	{

	}

	public virtual void OnHit (Transform hitBy, float hitback, Vector3 impactedNormal, Vector3 impactedPoint)
	{

	}
}
