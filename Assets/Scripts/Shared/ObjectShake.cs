using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
	public Transform shakedObject;
	public float duration;
	public float magnitude;
	Vector3 _originalLocalPosition;

	void Start ()
	{
		_originalLocalPosition = shakedObject.localPosition;
	}

	public void Shake ()
	{
		Shake (duration, magnitude);
	}

	public void Shake (float duration, float magnitude)
	{
		StartCoroutine (Shaking (duration, magnitude));
	}

	public IEnumerator Shaking (float duration, float magnitude)
	{
		// _originalLocalPosition = shakedObject.localPosition;
		var elapsed = 0f;
		while (elapsed <= 1f)
		{
			shakedObject.localPosition = _originalLocalPosition + Random.insideUnitSphere * magnitude;
			elapsed += Time.fixedDeltaTime / duration;
			yield return new WaitForFixedUpdate ();
		}
		shakedObject.localPosition = _originalLocalPosition;
	}
}
