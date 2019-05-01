using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
	public float forceMin;
	public float forceMax;

	Rigidbody _rigid;

	void Awake ()
	{
		_rigid = GetComponent<Rigidbody> ();
	}

	IEnumerator Start ()
	{
		var force = Random.Range (forceMin, forceMax);
		_rigid.AddForce (-transform.forward * force);
		_rigid.AddTorque (Random.insideUnitSphere * force);
		yield return new WaitForSeconds (.5f);
		Destroy (gameObject);
	}
}
