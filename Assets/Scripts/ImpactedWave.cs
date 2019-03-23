using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactedWave : MonoBehaviour
{
	// [System.NonSerialized]
	public LayerMask layerMask;
	// [System.NonSerialized]
	public float detectedDistance = .5f;
	// [System.NonSerialized]
	public float size = 2f;
	// [System.NonSerialized]
	public float capacity = 3f;
	// [System.NonSerialized]
	public Transform impactedObject;
	[System.NonSerialized]
	public Vector3 normal;
	[System.NonSerialized]
	public Vector3 impactedPoint;
	[System.NonSerialized]
	public bool free;
	[SerializeField]
	Transform _generatedPoint;
	CachedImpactEchoBeam _cachedImpactEchoBeam;
	float _deltaDistance;
	Vector3 _start;
	Vector3 _direction;

	void Awake ()
	{
		_cachedImpactEchoBeam = FindObjectOfType<CachedImpactEchoBeam> ();
		free = true;
		gameObject.SetActive (false);
	}

	void Start ()
	{
		// Init position of generation
		_generatedPoint.localPosition = Vector3.right * -detectedDistance;
	}

	public void Use ()
	{
		if (!free) return;
		free = false;
		gameObject.SetActive (true);
		transform.SetParent (impactedObject);
		// Init delta distance
		_deltaDistance = size / (capacity * 2);
		// Init the direction
		_direction = transform.rotation * Vector3.right;
		// Generation
		StartCoroutine (Generate ());
	}

	IEnumerator Generate ()
	{
		yield return null;
		var inx = 0;
		var inx1 = 0;
		var switchDir = false;
		while (inx < capacity * 2)
		{
			if (inx >= capacity && !switchDir)
			{
				_generatedPoint.localPosition = Vector3.right * -detectedDistance;
				inx1 = 0;
				switchDir = true;
			}
			var sign = inx >= capacity ? 1 : -1;
			var p1 = _generatedPoint.localPosition;
			p1.y = _deltaDistance * sign * inx1;
			_generatedPoint.localPosition = p1;
			var pos = _generatedPoint.position;
			var hit = Physics2D.Raycast (pos, _direction, detectedDistance + .5f, layerMask);
			if (hit && hit.distance > 0)
			{
				_cachedImpactEchoBeam.Use (hit.point, _direction, hit.normal);
			}
			// _cachedImpactEchoBeam.Use (transform.position, _direction, transform.position.normalized);
			++inx;
			++inx1;
			yield return new WaitForSeconds(.09f);
		}
		// Destroy (gameObject);
		free = true;
		transform.SetParent (null);
		gameObject.SetActive (false);
	}
}
