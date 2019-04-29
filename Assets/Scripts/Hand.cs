using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
	[Range (0, 2)]
	public float maximumRange = 1f;
	[SerializeField]
	Transform _arm;
	DotSight _dotSight;
	float _maximumDistance = 4f;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	void Start ()
	{
		transform.position = _arm.position;
	}

	void Update ()
	{
		MoveInRange ();
	}

	void MoveInRange ()
	{
		var distance = Vector3.Distance (_dotSight.GetPosition (), _arm.position);
		var rangeForMoving = distance / _maximumDistance;
		var pos = transform.localPosition;
		pos.x = Mathf.Clamp (rangeForMoving, 0, maximumRange);
		transform.localPosition = pos;
	}
}
