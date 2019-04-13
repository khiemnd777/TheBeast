using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilentPistol : Gun
{
	public float maxDistance;
	public float timeImpactAtMaxDistance;
	public float timeBetweenShoot;
	public Bullet bulletPrefab;

	public LayerMask layerMask;
	[SerializeField]
	AudioSource _audioSource;
	CachedEchoBeam _cachedEchoBeam;
	bool _availableHoldTrigger;
	float _timeAvailableHoleTrigger = 1f;

	public override void Awake ()
	{
		// _dotSight = FindObjectOfType<DotSight> ();
		_cachedEchoBeam = FindObjectOfType<CachedEchoBeam> ();
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
		if (!_availableHoldTrigger) yield break;
		// sound of being at launching bullet
		_cachedEchoBeam.Use (18, transform.position, 8, .175f, .35f, transform);
		_timeAvailableHoleTrigger = 0f;
		_availableHoldTrigger = false;
		var bulletIns = Instantiate<Bullet> (bulletPrefab, transform.position, transform.rotation);
		bulletIns.maxDistance = maxDistance;
		bulletIns.timeImpactAtMaxDistance = timeImpactAtMaxDistance;
		bulletIns.layerMask = layerMask;
		yield return new WaitForSeconds(.02f);
		_audioSource.Play();
	}
}
