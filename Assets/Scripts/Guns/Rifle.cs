using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{
	public float maxDistance;
	public float timeImpactAtMaxDistance;
	public float timeBetweenShoot;
	public RifleBullet bulletPrefab;
	public LayerMask layerMask;
	[SerializeField]
	Transform _projectile;
	[SerializeField]
	Animator _fireAnim;
	[SerializeField]
	Animator _flashAnim;
	[SerializeField]
	AudioSource _audioSource;

	bool _isHoldTrigger;
	bool _availableHoldTrigger;
	float _thetaProjectileAngle = 3f;
	float _timeAvailableHoldTrigger = 1f;
	float _timeBetweenHoldTrigger;
	float _t;

	public override void Awake ()
	{
		// _dotSight = FindObjectOfType<DotSight> ();
	}

	public override void Update ()
	{
		if (_timeAvailableHoldTrigger < 1f)
		{
			_timeAvailableHoldTrigger += Time.deltaTime / timeBetweenShoot;
		}
		if (_timeAvailableHoldTrigger >= 1f)
		{
			_availableHoldTrigger = true;
		}
	}

	Quaternion CalculateBulletQuaternion ()
	{
		// it's late +.1s?
		var angleRandom = Time.time - _timeBetweenHoldTrigger > timeBetweenShoot + Time.deltaTime ? 0 : _thetaProjectileAngle;
		var rot = _projectile.rotation;
		var rotAngle = rot.eulerAngles;
		var subRot = Quaternion.Euler (rotAngle.x, rotAngle.y + Random.Range (-angleRandom, angleRandom), rotAngle.z);
		_timeBetweenHoldTrigger = Time.time;
		return subRot;
	}

	public override void HoldTrigger ()
	{
		if (!_availableHoldTrigger) return;
		_timeAvailableHoldTrigger = 0f;
		_availableHoldTrigger = false;
		var bulletRot = CalculateBulletQuaternion ();
		var bulletIns = Instantiate<RifleBullet> (bulletPrefab, _projectile.position, bulletRot);
		bulletIns.maxDistance = maxDistance;
		bulletIns.timeImpactAtMaxDistance = timeImpactAtMaxDistance;
		bulletIns.layerMask = layerMask;
		if (OnProjectileLaunched != null)
		{
			OnProjectileLaunched ();
		}
		_flashAnim.Play ("Gun Flash", 0, 0);
		_fireAnim.Play ("Rifle Fire", 0, 0);
		// yield return new WaitForSeconds (.02f);
		// sound of being at launching bullet
		_audioSource.Play ();
		_isHoldTrigger = true;
	}

	public override void ReleaseTrigger ()
	{
		_isHoldTrigger = false;
	}
}
