using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ultrasound : MonoBehaviour
{
	public float speed;
	[SerializeField]
	Transform _echoBeamProjectile;
	CachedEchoBeam _cachedEchoBeam;
	TrailRenderer _trail;
	float _trailWidth;
	float _t;
	float _t2;
	float _t3;

	void Awake ()
	{
		_cachedEchoBeam = FindObjectOfType<CachedEchoBeam> ();
		_trail = GetComponent<TrailRenderer> ();
		_trailWidth = _trail.widthMultiplier;
	}

	void Start ()
	{
		_cachedEchoBeam.Use (36, _echoBeamProjectile.position, 4, .175f, 1);
	}

	void Update ()
	{
		transform.Translate (Vector2.right * Time.deltaTime * speed);

		//
		_t2 += Time.deltaTime / .75f;
		if (_t2 <= 1)
		{
			InstaintiateEchoBeam (36, 4, 1);
		}
		else
		{
			InstaintiateEchoBeam (18, 3, .625f);
			//
			_t3 += Time.deltaTime / .5f;
			if (_t3 <= 1f)
			{
				_trail.widthMultiplier = Mathf.Lerp (_trailWidth, 0f, _t3);
			}
			else
			{
				Destroy (gameObject);
			}
		}
	}

	void InstaintiateEchoBeam (int amount, float speed, float lifetime)
	{
		//
		_t += Time.deltaTime / .35f;
		if (_t >= 1)
		{
			_cachedEchoBeam.Use (amount, _echoBeamProjectile.position, speed, .175f, lifetime);
			_t = 0f;
		}
	}
}
