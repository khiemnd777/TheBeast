using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
	public float duration;
	public float magnitude;
	Vector3 _originalLocalPosition;

	void Start ()
	{
		_originalLocalPosition = transform.localPosition;
	}

	public void Shake ()
	{
		StartCoroutine (Shaking ());
	}

	public IEnumerator Shaking ()
	{
		// _originalLocalPosition = transform.localPosition;
		var elapsed = 0f;
		while (elapsed <= 1f)
		{
			transform.localPosition = _originalLocalPosition + Random.insideUnitSphere * magnitude;
			elapsed += Time.fixedDeltaTime / duration;
			yield return new WaitForFixedUpdate ();
		}
		transform.localPosition = _originalLocalPosition;
	}
}
