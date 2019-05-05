using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player2 : MonoBehaviour
{
	public float sprintSpeed = 1f;
	public float walkSpeed = .5f;
	public float sprintVolume = .09f;
	public float walkVolume = .01f;
	[SerializeField]
	Transform _body;
	[SerializeField]
	Transform _foots;
	[SerializeField]
	Transform _leftFoot;
	[SerializeField]
	Transform _rightFoot;
	[Space]
	[SerializeField]
	AudioSource _footstepSoundFx;
	IDictionary<string, bool> _lockControlList = new Dictionary<string, bool> ();
	DotSight _dotSight;
	Echo _echo;
	Vector3 _direction;
	Rigidbody _rigidbody;
	bool _isLeftFoot;
	bool _isMoving;
	bool _isStopping = true;
	float _timeFootOnGround;
	float _speed;
	Settings _settings;

	void Awake ()
	{
		_rigidbody = GetComponent<Rigidbody> ();
		_settings = FindObjectOfType<Settings> ();
		_dotSight = FindObjectOfType<DotSight> ();
	}

	void Start ()
	{
		_footstepSoundFx.volume = sprintVolume;
	}

	void Update ()
	{
		if (IsLocked ())
		{
			_speed = 0;
			return;
		}
		Rotate2 ();
		var x = Input.GetAxisRaw ("Horizontal");
		var y = Input.GetAxisRaw ("Vertical");
		_isMoving = x != 0 || y != 0;
		_direction = Utilities.AlterVector3 (_direction, x, y);
		// Sprint by default
		_speed = sprintSpeed;
		_footstepSoundFx.volume = sprintVolume;
		// Walk
		if (Input.GetKey (KeyCode.LeftShift))
		{
			_speed = walkSpeed;
			_footstepSoundFx.volume = walkVolume;
		}
		if (_isMoving)
		{
			// foot rotation
			_foots.rotation = Quaternion.LookRotation (Vector3.up, _direction);
			_isStopping = false;
			_timeFootOnGround += Time.deltaTime / (_settings.playerFootOnGroundDelta / _speed);
			if (_timeFootOnGround >= 1)
			{
				_isLeftFoot = !_isLeftFoot;
				_timeFootOnGround = 0f;
			}
		}
		else if (!_isStopping)
		{
			_isLeftFoot = !_isLeftFoot;
			_timeFootOnGround = 0f;
			_isStopping = true;
		}
	}

	void FixedUpdate ()
	{
		_rigidbody.velocity = _direction * _speed;
	}

	void Rotate2 ()
	{
		var normal = _dotSight.NormalizeFromPoint (transform.position);
		var rot = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		var rotation = Quaternion.Euler (0f, rot, 0f);
		_body.rotation = rotation;
	}

	public void RegisterLock (string name)
	{
		if (_lockControlList.ContainsKey (name)) return;
		_lockControlList.Add (name, false);
	}

	public void Lock (string name)
	{
		if (!_lockControlList.ContainsKey (name)) return;
		_lockControlList[name] = true;
	}

	public void Unlock (string name)
	{
		if (!_lockControlList.ContainsKey (name)) return;
		_lockControlList[name] = false;
	}

	public bool IsLocked ()
	{
		return _lockControlList.Values.Any (locked => locked);
	}
}
