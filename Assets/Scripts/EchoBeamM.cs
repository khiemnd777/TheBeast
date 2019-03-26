﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoBeamM : Beam
{
	public LayerMask layerMask;
	[SerializeField]
	ImpactEchoBeam _impactEchoBeamPrefab;
	[SerializeField]
	ImpactedWave _impactedWave;
	CachedImpactedEcho _cachedImpactedEcho;
	Vector3 _lastPosition;
	float _time;
	float _trailWidth;
	float _flairTime;
	float _flairTimeCount;

	void Awake ()
	{
		_cachedImpactedEcho = FindObjectOfType<CachedImpactedEcho> ();
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
		MoveAndReflect ();
		Disappear ();
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

	public override void Use (Vector2 pos)
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
		if (hit && hit.distance > 0)
		{
			var reflDir = Vector2.Reflect (dir.normalized, hit.normal);
			var rot = Mathf.Atan2 (reflDir.y, reflDir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3 (0, 0, rot);
			InstantiateImpactEcho (hit, layerMask);
		}
		_lastPosition = transform.position;
	}

	void InstantiateImpactEcho (RaycastHit2D hit, LayerMask layerMask)
	{
		_cachedImpactedEcho.Use (hit, layerMask);
	}
}
