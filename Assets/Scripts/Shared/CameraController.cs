﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Camera theCamera;
	public Transform target;
	public float smoothSpeed = .125f;
	public Vector3 offset;
	public BoxCollider bound;

	void Update()
	{
		var desiredPos = target.position + offset;
		var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
		transform.position = Utilities.CameraInBound(theCamera, bound, smoothedPos);
	}
}
