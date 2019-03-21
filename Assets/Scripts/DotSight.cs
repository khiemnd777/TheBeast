using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotSight : MonoBehaviour
{
	[SerializeField]
	Camera _theCamera;

	void Awake ()
	{
		Cursor.visible = false;
	}

	void Update ()
	{
		transform.position = Input.mousePosition;
	}

	public Vector3 GetPosition ()
	{
		return _theCamera.ScreenToWorldPoint (transform.position);
	}

	public Vector3 NormalizeFromPoint (Vector3 point)
	{
		var pos = GetPosition ();
		var dir = pos - point;
		dir.Normalize ();
		return dir;
	}
}
