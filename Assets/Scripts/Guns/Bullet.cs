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
	// CachedImpactedWave _cachedImpactedWave;
	CachedImpactedEcho _cachedImpactedEcho;
	float _targetDistance;
	BulletImpactEffect _bulletImpactFx;
	RaycastHit _raycastHit;
	CachedEchoBeam _cachedEchoBeam;
	Vector3 _direction;
	float _t;
	bool _isHitOnTarget;

	void Awake ()
	{
		_cachedEchoBeam = FindObjectOfType<CachedEchoBeam> ();
		_bulletImpactFx = GetComponent<BulletImpactEffect> ();
		// _cachedImpactedWave = FindObjectOfType<CachedImpactedWave> ();
		_cachedImpactedEcho = FindObjectOfType<CachedImpactedEcho> ();
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
			ActivateBulleImpactedFx(impactPoint, _raycastHit.normal);
			InstantiateImpactedEcho (impactPoint, layerMask);
			// Echo beams
			_cachedEchoBeam.Use (36, impactPoint, 6, .175f, .75f);
		}
		Destroy (gameObject);
	}

	void InstantiateImpactedWave (Vector3 impactPoint, LayerMask layerMask)
	{
		// var targetNormal = _raycastTarget.normal;
		// var impactedWaveAngle = 180f + Mathf.Atan2 (targetNormal.y, targetNormal.x) * Mathf.Rad2Deg;
		// var impactedWaveRot = Quaternion.Euler (0, 0, impactedWaveAngle);
		// var impactedWave = Instantiate<ImpactedWave> (_impactedWave, impactPoint, impactedWaveRot);
		// impactedWave.layerMask = layerMask;
		// impactedWave.impactedObject = _raycastTarget.transform;
		// impactedWave.impactedPoint = impactPoint;
		// _cachedImpactedWave.Use (_raycastTarget, layerMask);
	}

	void ActivateBulleImpactedFx (Vector3 impactPoint, Vector3 normal)
	{
		_bulletImpactFx.maxSpeed = 4.5f;
		_bulletImpactFx.lifetime = .125f;
		_bulletImpactFx.Use (impactPoint, normal);
	}

	void InstantiateImpactedEcho (Vector3 impactPoint, LayerMask layerMask)
	{
		// var targetNormal = _raycastTarget.normal;
		// var impactedWaveAngle = 180f + Mathf.Atan2 (targetNormal.y, targetNormal.x) * Mathf.Rad2Deg;
		// var impactedWaveRot = Quaternion.Euler (0, 0, impactedWaveAngle);
		// var impactedWave = Instantiate<ImpactedWave> (_impactedWave, impactPoint, impactedWaveRot);
		// impactedWave.layerMask = layerMask;
		// impactedWave.impactedObject = _raycastTarget.transform;
		// impactedWave.impactedPoint = impactPoint;
		_cachedImpactedEcho.Use (_raycastHit, layerMask);
	}
}
