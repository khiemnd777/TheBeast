using System.Collections.Generic;
 using System.Collections;
using UnityEngine;

public class ListenedSound : MonoBehaviour
{
	public float initRadius = 0f;
	public float radius = 3f;
	public float timeBlowUp = .25f;
	public Transform target;
	SphereCollider _sCollider;

	void Awake ()
	{
		_sCollider = GetComponent<SphereCollider> ();
	}

	void Start ()
	{
		_sCollider.isTrigger = true;
		_sCollider.radius = initRadius;
	}

	public void Launch ()
	{
		transform.position = target.position;
		StartCoroutine (BlowUp ());
	}

	IEnumerator BlowUp ()
	{
		var p = 0f;
		while (p <= 1f)
		{
			p += Time.deltaTime / timeBlowUp;
			_sCollider.radius = Mathf.Lerp (initRadius, radius, p);
			yield return null;
		}
		_sCollider.radius = initRadius;
	}
}
