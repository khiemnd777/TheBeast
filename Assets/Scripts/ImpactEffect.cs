using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
	public float speed;
	public float lifetime;

	TrailRenderer _trail;
	float _time1;
	float _time2;
	Color _color;

	void Awake ()
	{
		_trail = GetComponent<TrailRenderer> ();
	}

	void Start ()
	{
		_color = _trail.materials[0].color;
	}

	void Update ()
	{
		Move ();
	}

	void Move ()
	{
		_time1 += Time.deltaTime / lifetime;
		if (_time1 <= 1f)
		{
			transform.Translate (Vector3.right * Time.deltaTime * speed);
		}
		else
		{
			_time2 += Time.deltaTime / .1f;
			if (_time2 <= 1f)
			{
				_trail.materials[0].color = new Color (_color.r, _color.g, _color.b, Mathf.Lerp (1, 0, _time2));
			}
			else
			{
				Destroy (gameObject);
			}
		}
	}
}
