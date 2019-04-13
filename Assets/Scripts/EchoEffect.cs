using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
public float speed;
public float lifetime;
public LayerMask layerMask;

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
	// _trail.time = lifetime;
	// Destroy(gameObject, lifetime + _trail.time);
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
		// var dir = transform.position - _lastPosition;
		// var hit = Physics2D.Raycast (transform.position, dir.normalized, .175f, layerMask);
		// if (hit.collider != null && hit.distance > 0)
		// {
		// 	var reflDir = Vector2.Reflect (dir.normalized, hit.normal);
		// 	var rot = Mathf.Atan2 (reflDir.y, reflDir.x) * Mathf.Rad2Deg;
		// 	transform.eulerAngles = new Vector3 (0, 0, rot);
		// }
		// _lastPosition = transform.position;
	}
	else
	{
		_time2 += Time.deltaTime / .25f;
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
