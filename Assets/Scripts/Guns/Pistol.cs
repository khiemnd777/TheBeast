using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
	public float maxDistance;
	public float timeImpactAtMaxDistance;
	public float timeBetweenShoot;
	public Bullet bulletPrefab;
	public LayerMask layerMask;
	[SerializeField]
	Transform _projectile;
	[SerializeField]
	Animator _flashAnim;
	[SerializeField]
	AudioSource _audioSource;
	bool _isHoldTrigger;
	bool _availableHoldTrigger;
	float _timeAvailableHoleTrigger = 1f;

	public override void Awake ()
	{
		// _dotSight = FindObjectOfType<DotSight> ();
	}

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

	public override IEnumerator HoldTrigger ()
	{
		if(_isHoldTrigger) yield break;
		if (!_availableHoldTrigger) yield break;
		// sound of being at launching bullet
		_timeAvailableHoleTrigger = 0f;
		_availableHoldTrigger = false;
		var bulletIns = Instantiate<Bullet> (bulletPrefab, _projectile.position, _projectile.rotation);
		bulletIns.maxDistance = maxDistance;
		bulletIns.timeImpactAtMaxDistance = timeImpactAtMaxDistance;
		bulletIns.layerMask = layerMask;
		if (OnProjectileLaunched != null)
		{
			OnProjectileLaunched ();
		}
		_flashAnim.Play ("Gun Flash", 0, 0);
		yield return new WaitForSeconds (.02f);
		_audioSource.Play ();
		_isHoldTrigger = true;
	}

	public override void ReleaseTrigger ()
	{
		_isHoldTrigger = false;
	}
}
