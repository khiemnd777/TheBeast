using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactedEcho : MonoBehaviour
{
	public float speed = 2f;
	public float size = .5f;
	// [System.NonSerialized]
	public LayerMask layerMask;
	// [System.NonSerialized]
	public float detectedDistance = .5f;
	// [System.NonSerialized]
	public Transform impactedObject;
	[System.NonSerialized]
	public Vector3 normal;
	[System.NonSerialized]
	public Vector3 impactedPoint;
	[System.NonSerialized]
	public bool free;
	[System.NonSerialized]
	public RaycastHit hit;
	[SerializeField]
	Transform _generatedPoint;
	[SerializeField]
	Transform _beamPoint;
	[SerializeField]
	Transform _beamPoint2;
	Vector3 _start;
	Vector3 _direction;
	[SerializeField]
	TrailRenderer _trail;
	[SerializeField]
	TrailRenderer _trail2;
	float _trailTime;
	float _deltaDistance;
	PlayerMovement _player;
	bool _firstTime;

	void Awake ()
	{
		free = true;
		gameObject.SetActive (false);
		_trailTime = _trail.time;
		_player = FindObjectOfType<PlayerMovement> ();
	}

	void Update ()
	{
		if (_firstTime)
		{
			_trail.time = _trailTime;
			_trail2.time = _trailTime;
			_firstTime = false;
		}
	}

	public void Use ()
	{
		if (!free) return;
		_trail.time = 0f;
		_trail2.time = 0f;
		_firstTime = true;
		// _generatedPoint.localPosition = Vector3.right * -detectedDistance;
		// _generatedPoint.position = _player.transform.position;
		// transform.SetParent (impactedObject);
		// transform.SetParent (_player.transform);
		transform.position = hit.point;
		var dir = hit.normal;
		var rot = 360f - 180f - Mathf.Atan2 (dir.z, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (0f, rot, 0f);
		_generatedPoint.localPosition = Vector3.right * -detectedDistance;
		// Init the direction
		_direction = transform.rotation * Vector3.right;
		// detectedDistance = (impactedPoint - _player.transform.position).magnitude;
		free = false;
		// Generation
		gameObject.SetActive (true);
		StartCoroutine (SlideBeam ());
	}

	void SlideBeamPoint (Vector3 pos, Transform beamPoint)
	{
		// Debug.DrawRay (pos, _direction * (detectedDistance + .5f), Color.yellow);
		Debug.DrawRay (pos, _direction * (1000), Color.yellow);
		RaycastHit hit;
		// if (Physics.Raycast (pos, _direction, out hit, detectedDistance + .5f, layerMask))
		if (Physics.Raycast (pos, _direction, out hit, 1000f, layerMask))
		{
			if (hit.transform.gameObject.GetInstanceID () != impactedObject.gameObject.GetInstanceID ()) return;
			beamPoint.transform.position = hit.point;
		}
	}

	void SlideBeamPoint (Transform origin, Transform beamPoint)
	{
		// Debug.DrawRay (pos, _direction * (detectedDistance + .5f), Color.yellow);
		var pos = origin.position;
		var dir = origin.rotation * Vector3.right;
		Debug.DrawRay (pos, dir * (detectedDistance + .5f), Color.yellow);
		RaycastHit hit;
		// if (Physics.Raycast (pos, _direction, out hit, detectedDistance + .5f, layerMask))
		if (Physics.Raycast (pos, dir, out hit, detectedDistance + .5f, layerMask))
		{
			// if (hit.transform.gameObject.GetInstanceID () != impactedObject.gameObject.GetInstanceID ()) return;
			beamPoint.transform.position = hit.point;
		}
	}

	IEnumerator SlideBeamSide (int side, Vector3 orginalPos, Transform beamPoint)
	{
		var pc = 0f;
		while (pc <= 1f)
		{
			pc += Time.deltaTime * speed;
			// _generatedPoint.localPosition = Vector3.Lerp (orginalPos, new Vector3 (orginalPos.x, orginalPos.y, orginalPos.z + size * side), pc);
			var angle = Vector3.Lerp (Vector3.zero, Vector3.up * 30f * side, pc);
			_generatedPoint.localRotation = Quaternion.Euler (angle);
			SlideBeamPoint (_generatedPoint, beamPoint);
			yield return null;
		}
	}

	IEnumerator SlideBeam ()
	{
		var orginalPos = _generatedPoint.localPosition;
		StartCoroutine (SlideBeamSide (1, orginalPos, _beamPoint));
		yield return StartCoroutine (SlideBeamSide (-1, orginalPos, _beamPoint2));
		free = true;
		// transform.SetParent (_player.transform);
		gameObject.SetActive (false);
	}
}
