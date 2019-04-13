using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBeamBAK : MonoBehaviour
{
	public float speed;
	public float distance;
	public LayerMask layerMask;
	public float lifetime;
	[System.NonSerialized]
	public bool free;
	[SerializeField]
	ImpactEchoBeam _insideEchoBeamPrefab;
	TrailRenderer _trailRenderer;
	Vector3 _lastPosition;
	float _time;
	float _vanishingTime;
	float _trailWidth;
	float _flairTime;
	float _flairTimeCount;

	void Awake ()
	{
		_trailRenderer = GetComponentInChildren<TrailRenderer> ();
		gameObject.SetActive (false);
		_trailWidth = _trailRenderer.startWidth;
		free = true;
	}

	void Start ()
	{
		_lastPosition = transform.position;
		_flairTime = lifetime / 10f;
	}

	void Update ()
	{
		if (free) return;
		MoveAndReflect ();
		Disappear ();
	}

	void Disappear ()
	{
		_time += Time.deltaTime / (lifetime * .7f);
		if (_time >= 1f)
		{
			_vanishingTime += Time.deltaTime / (lifetime * .3f);
			if (_vanishingTime <= 1f)
			{
				var currentTrailWidth = Mathf.Lerp (_trailWidth, 0, _vanishingTime);
				_trailRenderer.startWidth = currentTrailWidth;
			}
			else
			{
				// Destroy (gameObject);
				gameObject.SetActive (false);
				free = true;
			}
		}
	}

	void Flair ()
	{
		_flairTimeCount += Time.deltaTime / _flairTime;
		if (_flairTime <= 1f)
		{
			var currentTrailWidth = Mathf.Lerp (_trailWidth, 0, _flairTime);
			_trailRenderer.startWidth = currentTrailWidth;
		}
	}

	public void Use (Vector2 pos)
	{
		if (!free) return;
		transform.position = pos;
		_lastPosition = transform.position;
		_time = 0f;
		_vanishingTime = 0f;
		_trailRenderer.startWidth = _trailWidth;
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
			// Destroy(gameObject);
		}
		_lastPosition = transform.position;
	}

	void InstantiateImpactEcho (Vector3 direction, Vector2 normal)
	{
		var perpendicular = new Vector2 (normal.y, -normal.x);
		var fromAngle = Mathf.Atan2 (perpendicular.y, perpendicular.x) * Mathf.Rad2Deg;
		var amount = 1f;
		var deltaAngle = 180f / amount;
		for (var i = 0; i <= amount; i++)
		{
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, 0f, angle);
			Instantiate<ImpactEchoBeam> (_insideEchoBeamPrefab, transform.position, euler);
		}
	}
}
