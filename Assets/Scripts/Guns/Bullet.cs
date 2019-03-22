using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float timeImpactAtMaxDistance;
	public float maxDistance;
	public LayerMask layerMask;
	[SerializeField]
	ImpactedWave _impactedWave;
	float _targetDistance;
	BulletImpactEffect _bulletImpactFx;
	RaycastHit2D _raycastTarget;
	CachedEchoBeam _cachedEchoBeam;
	Vector3 _direction;
	float _t;

	void Awake ()
	{
		_cachedEchoBeam = FindObjectOfType<CachedEchoBeam> ();
		_bulletImpactFx = GetComponent<BulletImpactEffect> ();
	}

	void Start ()
	{
		_direction = transform.rotation * Vector2.right;
		// 
		var castHit = Physics2D.Raycast (transform.position, _direction, maxDistance, layerMask);
		if (castHit.collider != null && castHit.distance > 0)
		{
			_targetDistance = castHit.distance;
			_raycastTarget = castHit;
		}
		else
		{
			_targetDistance = maxDistance;
			_raycastTarget = castHit;
		}
	}

	void Update ()
	{
		//
		if (_t <= 1f)
		{
			var timeToImpact = timeImpactAtMaxDistance * _targetDistance / maxDistance;
			_t += Time.deltaTime / timeToImpact;
			return;
		}
		if (_raycastTarget)
		{
			var impactPoint = _raycastTarget.point;
			// Bullet impact fx
			_bulletImpactFx.maxSpeed = 4.5f;
			_bulletImpactFx.lifetime = .125f;
			_bulletImpactFx.Use (impactPoint, _raycastTarget.normal);
			// Impacted wave
			InstantiateImpactedWave (impactPoint);
			// Echo beams
			_cachedEchoBeam.Use (36, impactPoint, 6, .175f, .75f);
		}
		Destroy (gameObject);
	}

	void InstantiateImpactedWave (Vector3 impactPoint)
	{
		var targetNormal = _raycastTarget.normal;
		var impactedWaveAngle = 180f + Mathf.Atan2 (targetNormal.y, targetNormal.x) * Mathf.Rad2Deg;
		var impactedWaveRot = Quaternion.Euler (0, 0, impactedWaveAngle);
		var impactedWave = Instantiate<ImpactedWave> (_impactedWave, impactPoint, impactedWaveRot);
		impactedWave.layerMask = layerMask;
		impactedWave.impactedObject = _raycastTarget.transform;
		impactedWave.impactedPoint = impactPoint;
	}
}
