﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetKatana : NetMelee
{
  int _slashQueueIndex;
  BoxCollider _collider;
  [SerializeField]
  TrailRenderer _trail;
  [SerializeField]
  AnimationClip _commonStyleAnim;
  AnimationClip _currentSlashAnim;
  public List<AnimationClip> slashQueue;
  SlowMotionMonitor _slowMotionMonitor;
  CameraShake _cameraShake;
  float _startTriggerTime;
  float _endTriggerTime;

  public override void Awake()
  {
    base.Awake();
    _collider = GetComponent<BoxCollider>();
    _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor>();
    _cameraShake = FindObjectOfType<CameraShake>();
  }

  public override void Start()
  {
    player.locker.RegisterLock("Katana");
    if (!netIdentity.isLocal)
    {
      netIdentity.onMessageReceived += OnMessageReceived;
    }
  }

  public override IEnumerator HoldTrigger()
  {
    _startTriggerTime = Time.time;
    var _triggerDistanceTime = _startTriggerTime - _endTriggerTime;
    var resetFirstSlash = _triggerDistanceTime > .3f;
    if (resetFirstSlash)
    {
      _currentSlashAnim = slashQueue[0];
      _slashQueueIndex = 0;
    }
    else
    {
      ++_slashQueueIndex;
      if (_slashQueueIndex >= slashQueue.Count)
      {
        _slashQueueIndex = 0;
      }
      _currentSlashAnim = slashQueue[_slashQueueIndex];
    }
    // Katana trigger to server and another clients.
    netIdentity.EmitMessage("katana_trigger", new KatanaSlashJson
    {
      slashQueueIndex = _slashQueueIndex
    });
    base.player.locker.Lock("Katana");
    playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    anyAction = true;
    hand.enabled = false;
    _trail.enabled = false;
    playerAnimator.Play(_currentSlashAnim.name, 0);
    yield return new WaitForSeconds(_currentSlashAnim.length);
    _endTriggerTime = Time.time;
    anyAction = false;
    hand.enabled = true;
    _trail.enabled = false;
    base.player.locker.Unlock("Katana");
  }

  public override void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    base.TakeUpArm(holder, hand, handAnimator);
    playerAnimator.Play(_commonStyleAnim.name, 0);
  }

  void OnMessageReceived(string eventName, string message)
  {
    switch (eventName)
    {
      case "katana_trigger":
        {
          var dataJson = Utility.Deserialize<KatanaSlashJson>(message);
          StartCoroutine(OnKatanaTriggerAnim(dataJson.slashQueueIndex));
        }
        break;
      default:
        break;
    }
  }

  IEnumerator OnKatanaTriggerAnim(int slashQueueIndex)
  {
    anyAction = true;
    if (slashQueueIndex < slashQueue.Count)
    {
      if (netIdentity.isServer)
      {
        Debug.Log($"{netIdentity.clientId}'s katana anim on server");
      }
      playerAnimator = player.animator;
      playerAnimator.runtimeAnimatorController = meleeAnimatorController;
      _currentSlashAnim = slashQueue[slashQueueIndex];
      playerAnimator.Play(_currentSlashAnim.name, 0);
      yield return new WaitForSeconds(_currentSlashAnim.length);
    }
    anyAction = false;
  }

  public override void KeepInCover()
  {
    if (!netIdentity.isLocal)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    if (playerAnimator) playerAnimator.enabled = false;
    if (hand) hand.enabled = true;
    if (_trail) _trail.enabled = false;
    anyAction = false;
    base.KeepInCover();
  }

  void OnTriggerEnter(Collider other)
  {
    if (netIdentity.isServer)
    {
      Debug.Log($"Katana trigger with anyAction: {anyAction}, on {other}");
    }
    if (!anyAction) return;
    if (other)
    {
      if (netIdentity.isServer)
      {
        var otherPlayer = other.GetComponent<Player>();
        if (otherPlayer)
        {
          Debug.Log($"{otherPlayer.clientId} slashed by katana...");
          var impactedPositionNormalized = other.ClosestPointOnBounds(transform.position);
          var impactedPoint = impactedPositionNormalized;
          impactedPositionNormalized.Normalize();
          otherPlayer.OnHittingUp(damage, freezedTime, hitback, impactedPoint, impactedPositionNormalized, true);
        }
      }
      // var hitMonster = other.GetComponent<Monster>();
      // if (hitMonster)
      // {
      //   var contactPoint = other.ClosestPointOnBounds(transform.position);
      //   var dir = GetDirection();
      //   dir.Normalize();
      //   // dir = dir * holder.transform.localScale.z;
      //   hitMonster.OnHit(transform, hitback, dir, contactPoint);
      //   _slowMotionMonitor.Freeze(.45f, .2f);
      //   _cameraShake.Shake(.125f, .125f);
      //   return;
      // }

      // var reversedObject = other.GetComponent<ReversedObject>();
      // if (reversedObject)
      // {
      //   var dir = GetDirection();
      //   dir.Normalize();
      //   reversedObject.reversed = true;
      //   reversedObject.speed *= 1.25f;
      //   reversedObject.normal = dir; //* holder.transform.localScale.z;
      //   _slowMotionMonitor.Freeze(.0625f, .2f);
      //   return;
      // }

      // var monsterWeaponEntity = other.GetComponent<MonsterWeaponEntity>();
      // if (monsterWeaponEntity && monsterWeaponEntity.anyAction)
      // {
      //   var contactPoint = other.ClosestPointOnBounds(transform.position);
      //   var dir = player.transform.position - contactPoint;
      //   dir.Normalize();
      //   player.OnFendingOff(monsterWeaponEntity.knockbackForce, dir, contactPoint);
      //   _slowMotionMonitor.Freeze(.08f, .08f);
      //   return;
      // }
    }
  }
}

public struct KatanaSlashJson
{
  public int slashQueueIndex;
}