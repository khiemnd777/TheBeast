using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Enemy
{
	public float echoLifetime;
	[SerializeField]
	Transform _foots;
	[SerializeField]
	AudioSource _footstepSoundFx;
	[SerializeField]
	Echo _echoPrefab;
	Echo _insEcho;
	float _timeFootOnGround;

	public override void Awake ()
	{
		base.Awake ();
	}

	public override void Update ()
	{
		// These code below should be considered later on.
		// if (agent.velocity != Vector3.zero)
		// if (!(!agent.pathPending 
		// 	&& agent.remainingDistance <= agent.stoppingDistance 
		// 		&& (!agent.hasPath || agent.velocity.sqrMagnitude == 0)))
		// if(!(agent.remainingDistance != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == .5f))
		if (agent.velocity.sqrMagnitude >= .5f)
		{
			_timeFootOnGround += Time.deltaTime / (0.5f / initSpeed);
			if (_timeFootOnGround >= 1f)
			{
				InstantiateEcho ();
				_timeFootOnGround = 0f;
			}
		}
		base.Update ();
	}

	void InstantiateEcho ()
	{
		// Footsteps fx
		_footstepSoundFx.Play ();
		if (_insEcho == null || _insEcho is Object && !_insEcho.Equals (null))
		{
			_insEcho = Instantiate<Echo> (_echoPrefab, Vector3.zero, Quaternion.identity);
		}
		_insEcho.position = _foots.position;
		_insEcho.owner = transform;
		_insEcho.lifetime = echoLifetime;
		_insEcho.Launch ();
	}
}
