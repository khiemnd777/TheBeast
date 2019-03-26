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
		_generatedPoint.localPosition = Vector3.right * -detectedDistance;
		transform.SetParent (impactedObject);
		// Init the direction
		_direction = transform.rotation * Vector3.right;
		free = false;
		// Generation
		gameObject.SetActive (true);
		StartCoroutine (SlideBeam ());
	}

	void SlideBeamPoint (Vector3 pos, Transform beamPoint)
	{
		var hit = Physics2D.Raycast (pos, _direction, detectedDistance + .5f, layerMask);
		if (!hit || hit.distance <= 0) return;
		if (hit.transform.gameObject.GetInstanceID () != impactedObject.gameObject.GetInstanceID ()) return;
		beamPoint.transform.position = hit.point;
	}

	bool _firstTime;

	IEnumerator SlideBeamSide (int side, Vector3 orginalPos, Transform beamPoint)
	{
		var pc = 0f;
		while (pc <= 1f)
		{
			pc += Time.deltaTime * speed;
			_generatedPoint.localPosition = Vector3.Lerp (orginalPos, new Vector3 (orginalPos.x, orginalPos.y + size * side, orginalPos.z), pc);
			SlideBeamPoint (_generatedPoint.position, beamPoint);
			yield return null;
		}
	}

	IEnumerator SlideBeam ()
	{
		var orginalPos = _generatedPoint.localPosition;
		StartCoroutine (SlideBeamSide (1, orginalPos, _beamPoint));
		yield return StartCoroutine (SlideBeamSide (-1, orginalPos, _beamPoint2));
		free = true;
		transform.SetParent (_player.transform);
		gameObject.SetActive (false);
	}
}
