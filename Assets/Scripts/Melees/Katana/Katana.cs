using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	Player2 _player;
	Hand _hand;
	int _slashQueueIndex;
	BoxCollider _collider;
	[SerializeField]
	TrailRenderer _trail;
	[SerializeField]
	RuntimeAnimatorController _animatorController;
	[SerializeField]
	AnimationClip _commonStyleAnim;
	AnimationClip _currentSlashAnim;
	public List<AnimationClip> slashQueue;
	SlowMotionMonitor _slowMotionMonitor;
	CameraShake _cameraShake;
	Animator _playerAnimator;
	float _startTriggerTime;
	float _endTriggerTime;

	public override void Awake ()
	{
		base.Awake ();
		_collider = GetComponent<BoxCollider> ();
		_slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
		_cameraShake = FindObjectOfType<CameraShake> ();
		// StartQueuingSlashes ();
	}

	public override void Start ()
	{
		player.RegisterLock ("Kanata");
	}

	// void StartQueuingSlashes ()
	// {
	// 	slashQueue.AddRange (new [] { _slash2Anim, _slashAnim, _slash3Anim });
	// }

	public override IEnumerator HoldTrigger ()
	{
		_startTriggerTime = Time.time;
		var _triggerDistanceTime = _startTriggerTime - _endTriggerTime;
		var resetFirstSlash = _triggerDistanceTime >.3f;
		if (resetFirstSlash)
		{
			_currentSlashAnim = slashQueue[0];
			_slashQueueIndex = 0;
		}
		else
		{
			_currentSlashAnim = slashQueue[++_slashQueueIndex];
			if (_slashQueueIndex >= slashQueue.Count)
			{
				_slashQueueIndex = 0;
			}
		}
		player.Lock ("Kanata");
		_playerAnimator.runtimeAnimatorController = _animatorController;
		anyAction = true;
		_hand.enabled = false;
		_trail.enabled = false;
		_playerAnimator.Play (_currentSlashAnim.name, 0);
		yield return new WaitForSeconds (_currentSlashAnim.length);
		_endTriggerTime = Time.time;
		anyAction = false;
		_hand.enabled = true;
		_trail.enabled = false;
		player.Unlock ("Kanata");
	}

	public override void TakeUpArm (MeleeHolder holder, Hand hand, Animator handAnimator, Player2 player)
	{
		_hand = hand;
		_player = player;
		_playerAnimator = _player.animator;
		_playerAnimator.runtimeAnimatorController = _animatorController;
		base.holder = holder;
		_playerAnimator.Play (_commonStyleAnim.name, 0);
	}

	public override void KeepInCover ()
	{
		_playerAnimator.enabled = false;
		_hand.enabled = true;
		anyAction = false;
		_trail.enabled = false;
		base.KeepInCover ();
	}
}
