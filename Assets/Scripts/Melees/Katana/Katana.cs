using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	Player2 _player;
	Hand _hand;
	int _slashCount;
	BoxCollider _collider;
	[SerializeField]
	TrailRenderer _trail;
	[SerializeField]
	RuntimeAnimatorController _animatorController;
	[SerializeField]
	AnimationClip _slashAnim;
	[SerializeField]
	AnimationClip _slash2Anim;
	[SerializeField]
	AnimationClip _commonStyleAnim;
	AnimationClip _currentSlashAnim;

	SlowMotionMonitor _slowMotionMonitor;
	CameraShake _cameraShake;
	Animator _playerAnimator;

	public override void Awake ()
	{
		base.Awake ();
		_collider = GetComponent<BoxCollider> ();
		_slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
		_cameraShake = FindObjectOfType<CameraShake> ();
	}

	public override void Start ()
	{
		player.RegisterLock ("Kanata");
	}

	float _startTriggerTime;
	float _endTriggerTime;

	public override IEnumerator HoldTrigger ()
	{
		_startTriggerTime = Time.time;
		var _triggerDistanceTime = _startTriggerTime - _endTriggerTime;
		var resetFirstSlash = _triggerDistanceTime > .3f;
		if (resetFirstSlash)
		{
			_currentSlashAnim = _slash2Anim;
			_slashCount = 1;
		}
		else
		{
			_currentSlashAnim = _slashCount++ % 2 == 0 ? _slash2Anim : _slashAnim;
		}
		player.Lock ("Kanata");
		_playerAnimator.runtimeAnimatorController = _animatorController;
		anyAction = true;
		_hand.enabled = false;
		_trail.enabled = true;
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
