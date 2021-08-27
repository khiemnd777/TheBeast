using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetKatana : NetMelee
{
  BoxCollider _collider;

  [SerializeField]
  TrailRenderer _trail;
  AnimationClip _currentSlashAnim;
  public List<AnimationClip> slashQueue;
  SlowMotionMonitor _slowMotionMonitor;
  CameraShake _cameraShake;

  public override void Awake()
  {
    base.Awake();
    _collider = GetComponent<BoxCollider>();
  }

  public override void Start()
  {
    base.Start();
    if (netIdentity.isLocal)
    {
      _slowMotionMonitor = FindObjectOfType<SlowMotionMonitor>();
      _cameraShake = FindObjectOfType<CameraShake>();
    }
    if (netIdentity.isServer)
    {
      _trail.enabled = false;
    }
    player.locker.RegisterLock("Katana");
  }

  public override void OnBeforePlayAnimation()
  {
    base.OnBeforePlayAnimation();
    _trail.enabled = false;
  }

  public override void OnAfterPlayAnimation()
  {
    base.OnAfterPlayAnimation();
    _trail.enabled = false;
  }

  // public override IEnumerator HoldTrigger()
  // {
  //   _startTriggerTime = Time.time;
  //   var _triggerDistanceTime = _startTriggerTime - _endTriggerTime;
  //   var resetFirstSlash = _triggerDistanceTime > .3f;
  //   if (resetFirstSlash)
  //   {
  //     _currentSlashAnim = slashQueue[0];
  //     _slashQueueIndex = 0;
  //   }
  //   else
  //   {
  //     ++_slashQueueIndex;
  //     if (_slashQueueIndex >= slashQueue.Count)
  //     {
  //       _slashQueueIndex = 0;
  //     }
  //     _currentSlashAnim = slashQueue[_slashQueueIndex];
  //   }
  //   // Katana trigger to server and another clients.
  //   netIdentity.EmitMessage("katana_trigger", new KatanaSlashJson
  //   {
  //     slashQueueIndex = _slashQueueIndex
  //   });
  //   base.player.locker.Lock("Katana");
  //   playerAnimator.runtimeAnimatorController = meleeAnimatorController;
  //   anyAction = true;
  //   hand.enabled = false;
  //   _trail.enabled = false;
  //   playerAnimator.Play(_currentSlashAnim.name, 0);
  //   yield return new WaitForSeconds(_currentSlashAnim.length);
  //   _endTriggerTime = Time.time;
  //   anyAction = false;
  //   hand.enabled = true;
  //   _trail.enabled = false;
  //   base.player.locker.Unlock("Katana");
  // }

  public override void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    base.TakeUpArm(holder, hand, handAnimator);
    playerAnimator.Play(commonStyleAnim.name, 0);
  }

  IEnumerator OnKatanaTriggerAnim(int slashQueueIndex)
  {
    if (slashQueueIndex < slashQueue.Count)
    {
      _currentSlashAnim = slashQueue[slashQueueIndex];
      playerAnimator.Play(_currentSlashAnim.name, 0);
      yield return new WaitForSeconds(_currentSlashAnim.length);
    }
    anyAction = false;
    hand.enabled = true;
  }

  public override void KeepInCover()
  {
    if (_trail) _trail.enabled = false;
    base.KeepInCover();
  }

  void OnTriggerEnter(Collider other)
  {
    if (!anyAction) return;
    if (other)
    {
      if (netIdentity.isServer)
      {
        Debug.Log($"Katana trigger with anyAction: {anyAction}, on {other}");
      }
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