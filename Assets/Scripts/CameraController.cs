using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	Camera _theCamera;
	public Transform target;
	public float smoothSpeed = .125f;
	public Vector3 offset;
	public BoxCollider2D bound;

	void Awake ()
	{
		_theCamera = GetComponent<Camera> ();
	}

	void Update()
	{
		var desiredPos = target.position + offset;
		var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
		transform.position = Utilities.CameraInBound(_theCamera, bound, smoothedPos);
	}
}
