using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCylora : Monster
{
	public float refreshRate = 1f;
	public float speed = 3;
	public Animator animator;
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

	Player2 _player;
	Transform _playerTransform;
	NavMeshAgent _agent;
	float _tdt;
    bool _isStopMoving;
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

	void Awake ()
	{
		_agent = GetComponent<NavMeshAgent> ();
		_player = FindObjectOfType<Player2> ();
		_playerTransform = _player.transform;
	}

	void Start ()
	{
		StartCoroutine (LeadtoTarget ());
	}

	void Update ()
	{
		_agent.speed = speed;
		UpdateWingsPosition ();
		RotateTowards (_playerTransform);
	}

	void RotateTowards (Transform target)
	{
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
					_agent.SetDestination (_playerTransform.position);
					_tdt = 0f;
				}
			}
			yield return null;
		}
	}
}
