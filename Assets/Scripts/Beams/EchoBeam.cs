using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBeam : Beam
{
	public LayerMask layerMask;
	[SerializeField]
	ImpactEchoBeam _impactEchoBeamPrefab;
	[SerializeField]
	Transform _rayPoint;
	CachedImpactEchoBeam _cachedImpactEchoBeam;
	Vector3 _lastPosition;
	float _time;
	float _trailWidth;
	float _flairTime;
	float _flairTimeCount;
	float _targetDistance;
	float _t;
	float _maxDistance = 1000f;
	RaycastHit2D _raycastTarget;

	void Awake ()
	{
		_cachedImpactEchoBeam = FindObjectOfType<CachedImpactEchoBeam> ();
		gameObject.SetActive (false);
		free = true;
	}

	void Start ()
	{
		_lastPosition = transform.position;
	}

	void Update ()
	{
		if (free) return;
		_t += (Time.deltaTime) / (_targetDistance / speed);
		if (_t >= 1f)
		{
			if (_raycastTarget)
			{
				var dir = transform.rotation * Vector3.right;
				var reflDir = Vector2.Reflect (dir.normalized, _raycastTarget.normal);
				var rot = Mathf.Atan2 (reflDir.y, reflDir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3 (0, 0, rot);
				transform.position = _raycastTarget.point;
				InstantiateImpactEcho(dir.normalized, _raycastTarget.normal);
			}
			_t = 0f;
		}
		Disappear ();
	}

	void FixedUpdate ()
	{
		var dir = transform.rotation * Vector3.right;
		var castHit = Physics2D.Raycast (_rayPoint.position, dir.normalized, _maxDistance, layerMask);
		if (castHit.collider != null && castHit.distance > 0)
		{
			_targetDistance = castHit.distance;
			_raycastTarget = castHit;
		}
		else
		{
			_targetDistance = _maxDistance;
			_raycastTarget = castHit;
		}
	}

	void Disappear ()
	{
		_time += Time.deltaTime / (lifetime * .7f);
		if (_time >= 1f)
		{
			gameObject.SetActive (false);
			free = true;
		}
	}

	public override void Use (Vector3 pos)
	{
		if (!free) return;
		transform.position = pos;
		_lastPosition = transform.position;
		_time = 0f;
		gameObject.SetActive (true);
		free = false;
	}

	void MoveAndReflect ()
	{
		transform.Translate (Vector2.right * Time.deltaTime * speed);
		var dir = transform.position - _lastPosition;
		var hit = Physics2D.Raycast (transform.position, dir.normalized, distance, layerMask);
		if (hit.collider != null && hit.distance > 0)
		{
			var reflDir = Vector2.Reflect (dir.normalized, hit.normal);
			var rot = Mathf.Atan2 (reflDir.y, reflDir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3 (0, 0, rot);
			InstantiateImpactEcho (dir.normalized, hit.normal);
		}
		_lastPosition = transform.position;
	}

	void InstantiateImpactEcho (Vector3 direction, Vector2 normal)
	{
		_cachedImpactEchoBeam.Use (transform.position, direction, normal);
	}
}
