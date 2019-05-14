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
			return;
		}
		if (_isHitOnTarget)
		{
			var impactPoint = _raycastHit.point;
			var hitTransform = _raycastHit.transform;
			var agent = hitTransform.GetComponent<NavMeshAgent> ();
			if (agent)
			{
				var hitNormal = _raycastHit.normal;
				agent.velocity = -hitNormal * hitback;
			}
			var shakeObject = hitTransform.GetComponentInChildren<ObjectShake> ();
			if (shakeObject)
			{
				shakeObject.Shake ();
			}
			ActivateBulleImpactedFx(_raycastHit);
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
