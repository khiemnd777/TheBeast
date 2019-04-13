using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedListenedSoundArea : MonoBehaviour
{
	public float radius = 1f;
	// public float timeTakenTargetAway = 
	[System.NonSerialized]
	public Vector3 detectedPosition;
	SphereCollider _sCollider;

	void Awake ()
	{
		_sCollider = GetComponent<SphereCollider> ();
	}

	void Start ()
	{
		detectedPosition = Vector3.zero;
		_sCollider.isTrigger = true;
		_sCollider.radius = radius;
	}

	void Update ()
	{

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Listened Sound")
		{
			var listenedSoundTarget = other.GetComponent<ListenedSound>();
			detectedPosition = listenedSoundTarget.transform.position;
		}
	}
}
