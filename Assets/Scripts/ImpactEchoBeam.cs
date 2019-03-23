using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEchoBeam : MonoBehaviour
{
	[System.NonSerialized]
	public float speed;
	public float lifetime;
	[System.NonSerialized]
	public bool free;

	TrailRenderer _trailRenderer;

	float _time;
	float _time2;
	float _time3;
	float _trailWidth;
	float _trailTime;
	int _firstTime;
	bool lastPhase;

	Settings _settings;
	Color _color;

	void Awake ()
	{
		_settings = FindObjectOfType<Settings> ();
		speed = _settings.impactEchoBeamSpeed;
		lifetime = _settings.impactEchoBeamLifeTime;
		free = true;
		gameObject.SetActive (false);
		_trailRenderer = GetComponentInChildren<TrailRenderer> ();
		if (_trailRenderer != null)
		{
			_color = _trailRenderer.materials[0].color;
			_trailWidth = _trailRenderer.widthMultiplier;
			_trailRenderer.time = lifetime;
			_trailTime = _trailRenderer.time;
		}
	}

	void Update ()
	{
		if (free) return;
		if (_firstTime == 0)
		{
			if (_trailRenderer != null)
			{
				_trailRenderer.time = _trailTime;
			}
			++_firstTime;
		}
		Move ();
		Lifetime ();
	}

	void LateUpdate ()
	{
		if (lastPhase)
		{
			free = true;
			gameObject.SetActive (false);
		}
	}

	void Lifetime ()
	{
		// _time += Time.deltaTime / (lifetime * .5f);
		// if (_time <= 1f) { }
		// else
		// {
		// 	_time2 += Time.deltaTime / (lifetime * .3f);
		// 	if (_time2 <= 1f)
		// 	{
		// 		var currentTrailWidth = Mathf.Lerp (_trailWidth, 0, _time2);
		// 		_trailRenderer.widthMultiplier = currentTrailWidth;
		// 	}
		// 	else
		// 	{
		// 		Destroy (gameObject);
		// 		_time3 += Time.deltaTime / (lifetime * .2f);
		// 		if (_time3 <= 1f)
		// 		{
		// 			_trailRenderer.materials[0].color = new Color (_color.r, _color.g, _color.b, Mathf.Lerp (1, 0, _time3));
		// 		}
		// 		else
		// 		{
		// 			lastPhase = true;
		// 		}
		// 	}
		// }
		_time += Time.deltaTime / (lifetime);
		if (_time >= 1f)
		{
			lastPhase = true;
		}
	}

	public void Use (Vector2 pos)
	{
		if (!free) return;
		transform.position = pos;
		if (_trailRenderer != null)
		{
			_trailRenderer.time = 0;
			_trailRenderer.widthMultiplier = _trailWidth;
			_trailRenderer.materials[0].color = _color;
		}
		_time = 0f;
		_time3 = 0f;
		_time2 = 0f;
		_firstTime = 0;
		lastPhase = false;
		free = false;
		gameObject.SetActive (true);
	}

	void Move ()
	{
		transform.Translate (Vector2.right * Time.fixedDeltaTime * speed);
	}
}
