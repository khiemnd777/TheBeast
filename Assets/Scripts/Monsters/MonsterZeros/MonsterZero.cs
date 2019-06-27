using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterZero : Monster
{
	public float refreshRate = 1f;
	public float speed = 3;
	public Transform head;
	public float rotationSpeedOfHead = 10f;
	public Animator animator;
	[SerializeField]
	Blood _bloodPrefab;
	[SerializeField]
	Blood _wideBloodPrefab;
	NavMeshAgent _agent;
	ObjectShake _objectShake;
	Player2 _player;
	Transform _playerTransform;
	float _tdt;
	float _storedSpeed;
	bool _isStopMoving;

	public void StopMoving ()
	{
		if (!_isStopMoving)
		{
			_storedSpeed = speed;
		}
		speed = 0;
		_isStopMoving = true;
	}

	public void KeepMoving ()
	{
		speed = _storedSpeed;
		_isStopMoving = false;
	}

	public override void OnHit (Transform hitBy, float hitback, RaycastHit raycastHit)
	{
		var impactPoint = raycastHit.point;
		var hitTransform = raycastHit.transform;
		if (_agent)
		{
			_agent.velocity = Utilities.HitbackVelocity (_agent.velocity, -raycastHit.normal, hitback);
		}
		if (_objectShake)
		{
			_objectShake.Shake ();
		}
		// bleed out
		if (_bloodPrefab)
		{
			Utilities.BleedOutAtPoint (_bloodPrefab, raycastHit.normal, raycastHit.point);
		}
	}

	public override void OnHit (Transform hitBy, float hitback, Vector3 impactedNormal, Vector3 impactedPoint)
	{
		if (_agent)
		{
			_agent.velocity = Utilities.HitbackVelocity (_agent.velocity, impactedNormal, hitback);
		}
		if (_objectShake)
		{
			_objectShake.Shake ();
		}
		// bleed out
		if (_wideBloodPrefab)
		{
			Utilities.BleedOut (_wideBloodPrefab, head.rotation, impactedPoint);
		}
	}

	void Awake ()
	{
		_agent = GetComponent<NavMeshAgent> ();
		_objectShake = GetComponentInChildren<ObjectShake> ();
		_player = FindObjectOfType<Player2> ();
		_playerTransform = _player.transform;
	}

	void Start ()
	{
		_storedSpeed = speed;
		StartCoroutine (LeadtoTarget ());
	}

	void Update ()
	{
		_agent.speed = speed;
		RotateTowards (_playerTransform);
	}

	void RotateTowards (Transform target)
	{
		var normal = target.position - head.position;
		normal.Normalize ();
		var rot = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		var rotation = Quaternion.Euler (90, rot, 90);
		head.rotation = rotation;
	}

	IEnumerator LeadtoTarget ()
	{
		while (true)
		{
			if (_player)
			{
				_tdt += Time.deltaTime / refreshRate;
				if (_tdt >= 1f)
				{
					_agent.SetDestination (_playerTransform.position);
					_tdt = 0f;
				}
			}
			yield return null;
		}
	}
}
