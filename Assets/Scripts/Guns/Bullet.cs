using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
	public float timeImpactAtMaxDistance;
	public float hitback;
	public float maxDistance;
	public LayerMask layerMask;
	[SerializeField]
	TrailRenderer _trail;
	float _targetDistance;
	BulletImpactEffect _bulletImpactFx;
	RaycastHit _raycastHit;
	Vector3 _direction;
	float _t;
	bool _isHitOnTarget;

	void Awake ()
	{
		_bulletImpactFx = GetComponent<BulletImpactEffect> ();
	}

	void Start ()
	{
		_direction = transform.rotation * Vector3.right;
		Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.right) * maxDistance, Color.yellow);
	}

	void Update ()
	{
		if (Physics.Raycast (transform.position, _direction, out _raycastHit, maxDistance, layerMask))
		{
			_targetDistance = _raycastHit.distance;
			_isHitOnTarget = true;
		}
		else
		{
			_targetDistance = maxDistance;
			_isHitOnTarget = false;
		}
		//
		if (_t <= 1f)
		{
			var timeToImpact = timeImpactAtMaxDistance * _targetDistance / maxDistance;
			_t += Time.deltaTime / timeToImpact;
			// Trail goes straight along direction
			_trail.transform.localPosition = Vector3.Lerp (Vector3.zero, Vector3.right * _targetDistance, _t);
			return;
		}
		if (_isHitOnTarget)
		{
			var impactPoint = _raycastHit.point;
			var hitTransform = _raycastHit.transform;
			var agent = hitTransform.GetComponent<NavMeshAgent> ();
			var hitMonster = hitTransform.GetComponent<Monster> ();
			if (hitMonster)
			{
				hitMonster.OnHit (transform, hitback, _raycastHit);
			}
			ActivateBulleImpactedFx (_raycastHit);
		}
		Destroy (gameObject);
	}

	void ActivateBulleImpactedFx (RaycastHit hit)
	{
		_bulletImpactFx.maxSpeed = 4.5f;
		_bulletImpactFx.lifetime = .125f;
		_bulletImpactFx.Use (hit.point, hit.normal);
	}
}
