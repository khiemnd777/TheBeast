﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
	public float maxDistance;
	public float timeImpactAtMaxDistance;
	public float timeBetweenShoot;
	public ShotgunBullet bulletPrefab;
	public LayerMask layerMask;
	[SerializeField]
	Transform _projectile;
	[SerializeField]
	Animator _shotgunFireAnim;
	[SerializeField]
	Animator _flashAnim;
	[SerializeField]
	AudioSource _audioSource;

	bool _isHoldTrigger;
	bool _availableHoldTrigger;
	float _timeAvailableHoleTrigger = 1f;

	public override void Update ()
	{
		if (_timeAvailableHoleTrigger < 1f)
		{
			_timeAvailableHoleTrigger += Time.deltaTime / timeBetweenShoot;
		}
		if (_timeAvailableHoleTrigger >= 1f)
		{
			_availableHoldTrigger = true;
		}
	}

	public override void HoldTrigger ()
	{
		if (_isHoldTrigger) return;
		if (!_availableHoldTrigger) return;
		// sound of being at launching bullet
		_timeAvailableHoleTrigger = 0f;
		_availableHoldTrigger = false;
		var bulletIns = Instantiate<ShotgunBullet> (bulletPrefab, _projectile.position, _projectile.rotation);
		bulletIns.maxDistance = maxDistance;
		bulletIns.timeImpactAtMaxDistance = timeImpactAtMaxDistance;
		bulletIns.layerMask = layerMask;
		if (OnProjectileLaunched != null)
		{
			OnProjectileLaunched ();
		}
		_flashAnim.Play ("Gun Flash", 0, 0);
		_shotgunFireAnim.Play ("Shotgun Fire", 0, 0);
		EjectShell ();
		// yield return new WaitForSeconds (.02f);
		_audioSource.Play ();
		_isHoldTrigger = true;
	}

	public override void ReleaseTrigger ()
	{
		_isHoldTrigger = false;
	}
}
