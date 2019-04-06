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
		var mp = _theCamera.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector3 (mp.x, 0f, mp.z);
	}

	public Vector3 GetPosition ()
	{
		return transform.position;
	}

	public Vector3 NormalizeFromPoint (Vector3 point)
	{
		var pos = GetPosition ();
		var dir = pos - point;
		dir.Normalize ();
		return dir;
	}
}
