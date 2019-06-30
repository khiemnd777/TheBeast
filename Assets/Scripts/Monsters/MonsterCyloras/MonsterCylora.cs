using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCylora : Monster
{
	public float refreshRate = 1f;
	public float speed = 3;
	public Animator animator;
	public bool blocked;
	[SerializeField]
	Transform _head;
	[SerializeField]
	Transform _wing1;
	[SerializeField]
	Transform _wingAnchor1;
	[SerializeField]
	Transform _wing2;
	[SerializeField]
	Transform _wingAnchor2;
	[SerializeField]
	Transform _wing3;
	[SerializeField]
	Transform _wingAnchor3;
	[SerializeField]
	Transform _wing4;
	[SerializeField]
	Transform _wingAnchor4;
	[SerializeField]
	Blood _bloodPrefab;
	[SerializeField]
	Blood _wideBloodPrefab;
	Player2 _player;
	Transform _playerTransform;
	[System.NonSerialized]
	public NavMeshAgent agent;
	ObjectShake _objectShake;
	bool _isStopMoving;
	bool _isStopRotating;
	float _tdt;
	float _storedSpeed;

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

	public void KeepMoving (float speed)
	{
		if (this.speed != 0 && this.speed != speed)
		{
			_storedSpeed = this.speed;
		}
		this.speed = speed;
		_isStopMoving = false;
	}

	public void SetVelocity (Vector3 velocity)
	{
		agent.velocity = velocity;
	}

	public void StopRotatingToTarget ()
	{
		_isStopRotating = true;
	}

	public void KeepRotatingToTarget ()
	{
		_isStopRotating = false;
	}

	public void StopLeadingToTarget ()
	{
		agent.enabled = false;
	}

	public void KeepLeadingToTarget ()
	{
		agent.enabled = true;
	}

	public override void OnHit (Transform hitBy, float hitback, RaycastHit raycastHit)
	{
		if (blocked) return;
		var impactPoint = raycastHit.point;
		var hitTransform = raycastHit.transform;
		if (agent)
		{
			agent.velocity = Utilities.HitbackVelocity (agent.velocity, -raycastHit.normal, hitback);
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
		if (blocked) return;
		if (agent)
		{
			agent.velocity = Utilities.HitbackVelocity (agent.velocity, impactedNormal, hitback);
		}
		if (_objectShake)
		{
			_objectShake.Shake ();
		}
		// bleed out
		if (_wideBloodPrefab)
		{
			Utilities.BleedOut (_wideBloodPrefab, _head.rotation, impactedPoint);
		}
	}

	void Awake ()
	{
		agent = GetComponent<NavMeshAgent> ();
		_player = FindObjectOfType<Player2> ();
		_objectShake = GetComponentInChildren<ObjectShake> ();
		_playerTransform = _player.transform;
	}

	void Start ()
	{
		StartCoroutine (LeadtoTarget ());
	}

	void Update ()
	{
		agent.speed = speed;
		UpdateWingsPosition ();
		RotateTowards (_playerTransform);
	}

	void RotateTowards (Transform target)
	{
		if (_isStopRotating) return;
		var normal = target.position - _head.position;
		normal.Normalize ();
		var rot = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		var rotation = Quaternion.Euler (90, rot, 90);
		_head.rotation = rotation;
	}

	void UpdateWingsPosition ()
	{
		UpdateWingPosition (_wing1, _wingAnchor1);
		UpdateWingPosition (_wing2, _wingAnchor2);
		UpdateWingPosition (_wing3, _wingAnchor3);
		UpdateWingPosition (_wing4, _wingAnchor4);
	}

	void UpdateWingPosition (Transform wing, Transform anchor)
	{
		wing.position = anchor.position;
		var wingDir = (wing.position - transform.position);
		wingDir.Normalize ();
		wing.rotation = Utilities.RotateByNormal (wingDir, Vector3.up);
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
					if (agent.enabled)
					{
						agent.SetDestination (_playerTransform.position);
					}
					_tdt = 0f;
				}
			}
			yield return null;
		}
	}
}
