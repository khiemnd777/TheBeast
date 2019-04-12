using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float sprintSpeed = 1f;
	public float walkSpeed = .5f;
	public float sprintEchoLifetime = 1f;
	public float walkEchoLifetime = .5f;
	public float sprintVolume = .09f;
	public float walkVolume = .01f;
	public float sprintStamina;
	public float walkStamina;
	[SerializeField]
	Transform _body;
	[SerializeField]
	Transform _foots;
	[SerializeField]
	Transform _leftFoot;
	[SerializeField]
	Transform _rightFoot;
	[SerializeField]
	Echo _echoPrefab;
	[SerializeField]
	AudioSource _footstepSoundFx;
	GroundedFoot _groundedFoot;
	DotSight _dotSight;
	Stamina _stamina;

	Vector3 _direction;
	Rigidbody _rigidbody;
	bool _isLeftFoot;
	bool _isMoving;
	bool _isStopping = true;
	float _timeFootOnGround;
	float _speed;
	float _echoLifetime;
	float _staminaConsume;
	Settings _settings;

	void Awake ()
	{
		_rigidbody = GetComponent<Rigidbody> ();
		_groundedFoot = GetComponent<GroundedFoot> ();
		_settings = FindObjectOfType<Settings> ();
		_dotSight = FindObjectOfType<DotSight> ();
		_stamina = FindObjectOfType<Stamina> ();
	}

	void Start ()
	{
		_footstepSoundFx.volume = sprintVolume;
		_echoLifetime = sprintEchoLifetime;
		_staminaConsume = sprintStamina;
	}

	void Update ()
	{
		Rotate2 ();
		// If the stamina was exceeded, the player then be stopped for restoring.
		if (_stamina.isExceeded)
		{
			_direction = Vector3.zero;
			return;
		}
		var x = Input.GetAxisRaw ("Horizontal");
		var y = Input.GetAxisRaw ("Vertical");
		_isMoving = x != 0 || y != 0;
		_direction = Utilities.AlterVector3(_direction, x, y);
		// Sprint by default
		_speed = sprintSpeed;
		_footstepSoundFx.volume = sprintVolume;
		_echoLifetime = sprintEchoLifetime;
		_staminaConsume = sprintStamina;
		// Walk
		if (Input.GetKey (KeyCode.LeftShift))
		{
			_speed = walkSpeed;
			_footstepSoundFx.volume = walkVolume;
			_echoLifetime = walkEchoLifetime;
			_staminaConsume = walkStamina;
		}
		if (_isMoving)
		{
			// foot rotation
			_foots.rotation = Quaternion.LookRotation (Vector3.up, _direction);
			_isStopping = false;
			_timeFootOnGround += Time.deltaTime / (_settings.playerFootOnGroundDelta / _speed);
			if (_timeFootOnGround >= 1)
			{
				// consume the stamina
				_stamina.Consume (_staminaConsume, () =>
				{
					// generate the echo
					InstantiateEcho ();
				});
				_isLeftFoot = !_isLeftFoot;
				_timeFootOnGround = 0f;
			}
		}
		else if (!_isStopping)
		{
			if (_timeFootOnGround < 1f)
			{
				// consume the stamina
				_stamina.Consume (_staminaConsume, () =>
				{
					// generate the echo
					InstantiateEcho ();
				});
			}
			_isLeftFoot = !_isLeftFoot;
			_timeFootOnGround = 0f;
			_isStopping = true;
		}
	}

	void InstantiateEcho ()
	{
		// Footsteps fx
		_footstepSoundFx.Play ();
		//
		var footPosition = _isLeftFoot ? _leftFoot : _rightFoot;
		_groundedFoot.Launch (footPosition.position);
		var echo = Instantiate<Echo> (_echoPrefab, footPosition.position, Quaternion.identity);
		echo.owner = transform;
		echo.lifetime = _echoLifetime;
		Destroy (echo.gameObject, echo.lifetime + .1f);
	}

	void FixedUpdate ()
	{
		_rigidbody.velocity = _direction * _speed;
	}

	void Rotate2 ()
	{
		var normal = _dotSight.NormalizeFromPoint(transform.position);
		var rot = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
		var rotation = Quaternion.Euler (0f, rot, 0f);
		_body.rotation = rotation;
	}
}
