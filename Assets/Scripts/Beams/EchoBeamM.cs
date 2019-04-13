using System.Collections;
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
		transform.Translate (Vector3.right * Time.deltaTime * speed);
		var dir = transform.position - _lastPosition;
		Debug.DrawRay (transform.position, dir.normalized * distance, Color.yellow);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, dir.normalized, out hit, distance, layerMask))
		{
			if (hit.distance > 0)
			{
				// if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("Enemy"))
				// {
				// 	var detectedArea = hit.transform.Find ("Detected Area");
				// 	if (detectedArea != null && detectedArea.gameObject.layer == LayerMask.NameToLayer ("Detected Area"))
				// 	{
				// 		var detectedEnemy = hit.transform.gameObject.GetComponent<Enemy> ();
				// 		if (detectedEnemy != null && detectedEnemy is Object && !detectedEnemy.Equals (null))
				// 		{
				// 			detectedEnemy.target = owner;
				// 		}
				// 	}
				// 	var reflDir = Vector3.Reflect (dir.normalized, hit.normal);
				// 	var rot = 360f - Mathf.Atan2 (reflDir.z, reflDir.x) * Mathf.Rad2Deg;
				// 	transform.eulerAngles = new Vector3 (0f, rot, 0f);
				// 	InstantiateImpactEcho (hit, layerMask);
				// }
				// else
				// {
				// 	var reflDir = Vector3.Reflect (dir.normalized, hit.normal);
				// 	var rot = 360f - Mathf.Atan2 (reflDir.z, reflDir.x) * Mathf.Rad2Deg;
				// 	transform.eulerAngles = new Vector3 (0f, rot, 0f);
				// 	InstantiateImpactEcho (hit, layerMask);
				// }
				var reflDir = Vector3.Reflect (dir.normalized, hit.normal);
				var rot = 360f - Mathf.Atan2 (reflDir.z, reflDir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3 (0f, rot, 0f);
				InstantiateImpactEcho (hit, layerMask);
			}
		}
		_lastPosition = transform.position;
	}

	void InstantiateImpactEcho (RaycastHit hit, LayerMask layerMask)
	{
		_cachedImpactedEcho.Use (hit, layerMask);
	}
}
