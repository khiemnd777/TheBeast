using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ShotgunBullet : MonoBehaviour
{
	public float initBallNumber = 5;
	public float hitback = 1f;
	public float timeImpactAtMaxDistance;
	public float maxDistance;
	public LayerMask layerMask;
	float _targetDistance;
	BulletImpactEffect _bulletImpactFx;
	List<RaycastHit> _raycastHits = new List<RaycastHit> ();
	List<float> _targetDistances = new List<float> ();
	List<float> _ts = new List<float> ();
	List<bool> _isHitOnTargets = new List<bool> ();
	List<Vector3> _directions = new List<Vector3> ();
	Vector3 _direction;
	float _t;
	bool _isHitOnTarget;

	void Awake ()
	{
		_bulletImpactFx = GetComponent<BulletImpactEffect> ();
	}

	void Start ()
	{
		// init delta times
		for (var i = 0; i < initBallNumber; i++)
		{
			_ts.Add (0);
		}
		_direction = transform.rotation * Vector3.right;
		// init directions
		var rot = transform.rotation;
		var rotAngle = rot.eulerAngles;
		for (var i = 0; i < initBallNumber; i++)
		{
			var subRot = Quaternion.Euler (rotAngle.x, rotAngle.y + Random.Range (-10, 10), rotAngle.z);
			var dir = subRot * Vector3.right;
			_directions.Add (dir);
			Debug.DrawRay (transform.position, dir * maxDistance, Color.yellow);
		}
		// Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.right) * maxDistance, Color.yellow);
	}

	void Update ()
	{
		_raycastHits.Clear ();
		_targetDistances.Clear ();
		_isHitOnTargets.Clear ();
		for (var i = 0; i < initBallNumber; i++)
		{
			RaycastHit raycastHit;
			var direction = _directions[i];
			if (Physics.Raycast (transform.position, direction, out raycastHit, maxDistance, layerMask))
			{
				_raycastHits.Add (raycastHit);
				_targetDistances.Add (raycastHit.distance);
				_isHitOnTargets.Add (true);
			}
			else
			{
				_raycastHits.Add (new RaycastHit ());
				_targetDistances.Add (maxDistance);
				_isHitOnTargets.Add (false);
			}
		}
		//
		for (var i = 0; i < initBallNumber; i++)
		{
			var t = _ts[i];
			var targetDistance = _targetDistances[i];
			if (t <= 1f)
			{
				var timeToImpact = timeImpactAtMaxDistance * targetDistance / maxDistance;
				t += Time.deltaTime / timeToImpact;
				_ts[i] = t;
				if (t < 1f) continue;
			}
			var isHitOnTarget = _isHitOnTargets[i];
			if (isHitOnTarget)
			{
				var raycastHit = _raycastHits[i];
				var hitTransform = raycastHit.transform;
				var hitMonster = hitTransform.GetComponent<Monster> ();
				if (hitMonster)
				{
					hitMonster.OnHit (transform, hitback, raycastHit);
				}
				ActivateBulleImpactedFx (raycastHit);
			}
		}
		if (_ts.All (x => x >= 1f))
		{
			Destroy (gameObject);
		}
	}

	void ActivateBulleImpactedFx (RaycastHit hit)
	{
		ActivateBulleImpactedFx(hit.point, hit.normal);
	}

	void ActivateBulleImpactedFx (Vector3 hitPoint, Vector3 hitNormal)
	{
		_bulletImpactFx.maxSpeed = 4.5f;
		_bulletImpactFx.lifetime = .125f;
		_bulletImpactFx.Use (hitPoint, hitNormal);
	}
}
