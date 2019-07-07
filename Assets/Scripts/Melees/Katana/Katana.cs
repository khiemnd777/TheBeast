﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Melee
{
	Player2 _player;
	Hand _hand;
	Animator _handAnimator;
	int _slashCount;
	BoxCollider _collider;
	[SerializeField]
	TrailRenderer _trail;
	[SerializeField]
	KatanaSlashEffect _slashEffectPrefab;
	[SerializeField]
	KatanaSlashEffect _slash2EffectPrefab;

	SlowMotionMonitor _slowMotionMonitor;
	CameraShake _cameraShake;

	public override void Awake ()
	{
		base.Awake ();
		_collider = GetComponent<BoxCollider> ();
		_slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
		_cameraShake = FindObjectOfType<CameraShake> ();
	}

	public override void Start ()
	{
		base.Start ();
	}

	public override IEnumerator HoldTrigger ()
	{
		if (anyAction) yield break;
		anyAction = true;
		_hand.enabled = false;
		_handAnimator.enabled = true;
		var slashAnimName = _slashCount++ % 2 == 0 ? "Katana Slash" : "Katana Slash 2";
		if (_slashCount % 2 == 0)
		{
			InstantiateSlashEffect (_slash2EffectPrefab);
		}
		else
		{
			InstantiateSlashEffect (_slashEffectPrefab);
		}
		_handAnimator.Play (slashAnimName, 0, 0);
		yield return StartCoroutine (EndOfAnimation ());
	}

	public override void TakeUpArm (MeleeHolder holder, Hand hand, Animator handAnimator, Player2 player)
	{
		_hand = hand;
		_handAnimator = handAnimator;
		_player = player;
		base.holder = holder;
	}

	public override void KeepInCover ()
	{
		_handAnimator.enabled = false;
		_hand.enabled = true;
		anyAction = false;
		_trail.enabled = false;
		base.KeepInCover ();
	}

	IEnumerator EndOfAnimation ()
	{
		_trail.enabled = true;
		var currentAnimatorStateInfo = _handAnimator.GetCurrentAnimatorStateInfo (0);
		yield return new WaitForSeconds (currentAnimatorStateInfo.length);
		_handAnimator.enabled = false;
		_hand.enabled = true;
		anyAction = false;
		_trail.enabled = false;
	}

	void InstantiateSlashEffect (KatanaSlashEffect fx)
	{
		var ins = Instantiate<KatanaSlashEffect> (fx, _player.transform.position, Quaternion.identity, _player.transform);
		ins.transform.localPosition = new Vector3 (1.08f, 0f, 0f);
		var dir = _player.body.transform.rotation * Vector3.right;
		dir.Normalize ();
		ins.transform.rotation = Utilities.RotateByNormal (dir, Vector3.up);
		ins.katana = this;
	}

	void OnTriggerEnter (Collider other)
	{
		if (!anyAction) return;
		if (other)
		{
			var hitMonster = other.GetComponent<Monster> ();
			if (hitMonster)
			{
				var contactPoint = other.ClosestPointOnBounds (transform.position);
				var dir = GetDirection ();
				dir.Normalize ();
				// dir = dir * holder.transform.localScale.z;
				hitMonster.OnHit (transform, hitback, dir, contactPoint);
				_slowMotionMonitor.Freeze (.45f, .2f);
				_cameraShake.Shake (.125f, .125f);
				return;
			}
			var reversedObject = other.GetComponent<ReversedObject> ();
			if (reversedObject)
			{
				var dir = GetDirection ();
				dir.Normalize ();
				reversedObject.reversed = true;
				reversedObject.speed *= 1.25f;
				reversedObject.normal = dir; //* holder.transform.localScale.z;
				_slowMotionMonitor.Freeze (.0625f, .2f);
				return;
			}
			var monsterWeaponEntity = other.GetComponent<MonsterWeaponEntity> ();
			if (monsterWeaponEntity && monsterWeaponEntity.anyAction)
			{
				var contactPoint = other.ClosestPointOnBounds (transform.position);
				var dir = _player.transform.position - contactPoint;
				dir.Normalize ();
				_player.OnFendingOff (monsterWeaponEntity.knockbackForce, dir, contactPoint);
				_slowMotionMonitor.Freeze (.08f, .08f);
				return;
			}
		}
	}
}