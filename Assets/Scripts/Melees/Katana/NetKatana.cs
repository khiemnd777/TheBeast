using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetKatana : NetMelee
{
  NetHand _hand;
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
  Animator _playerAnimator;
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
    player.locker.RegisterLock("Kanata");
    netIdentity.onMessageReceived += OnMessageReceived;
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
    base.player.locker.Lock("Kanata");
    _playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    anyAction = true;
    _hand.enabled = false;
    _trail.enabled = false;
    _playerAnimator.Play(_currentSlashAnim.name, 0);
    yield return new WaitForSeconds(_currentSlashAnim.length);
    _endTriggerTime = Time.time;
    anyAction = false;
    _hand.enabled = true;
    _trail.enabled = false;
    base.player.locker.Unlock("Kanata");
  }

  public override void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    _hand = hand;
    _playerAnimator = player.animator;
    _playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    base.holder = holder;
    _playerAnimator.Play(_commonStyleAnim.name, 0);
    // Emit after taking-up arm
    netIdentity.EmitMessage("katana_take_up_arm", null);
  }

  void OnMessageReceived(string eventName, string message)
  {
    switch (eventName)
    {
      case "katana_take_up_arm":
        {
          _playerAnimator = player.animator;
          _playerAnimator.runtimeAnimatorController = meleeAnimatorController;
        }
        break;
      case "katana_trigger":
        {
          var dataJson = Utility.Deserialize<KatanaSlashJson>(message);
          if (dataJson.slashQueueIndex < slashQueue.Count)
          {
            _currentSlashAnim = slashQueue[dataJson.slashQueueIndex];
            _playerAnimator.runtimeAnimatorController = meleeAnimatorController;
            _playerAnimator.Play(_currentSlashAnim.name, 0);
          }
        }
        break;
      default:
        break;
    }
  }

  public override void KeepInCover()
  {
    // _playerAnimator.enabled = false;
    _hand.enabled = true;
    anyAction = false;
    _trail.enabled = false;
    if (netIdentity.isClient)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    base.KeepInCover();
  }

  void OnTriggerEnter(Collider other)
  {
    if (!anyAction) return;
    if (other)
    {
      var hitMonster = other.GetComponent<Monster>();
      if (hitMonster)
      {
        var contactPoint = other.ClosestPointOnBounds(transform.position);
        var dir = GetDirection();
        dir.Normalize();
        // dir = dir * holder.transform.localScale.z;
        hitMonster.OnHit(transform, hitback, dir, contactPoint);
        _slowMotionMonitor.Freeze(.45f, .2f);
        _cameraShake.Shake(.125f, .125f);
        return;
      }
      var reversedObject = other.GetComponent<ReversedObject>();
      if (reversedObject)
      {
        var dir = GetDirection();
        dir.Normalize();
        reversedObject.reversed = true;
        reversedObject.speed *= 1.25f;
        reversedObject.normal = dir; //* holder.transform.localScale.z;
        _slowMotionMonitor.Freeze(.0625f, .2f);
        return;
      }
      var monsterWeaponEntity = other.GetComponent<MonsterWeaponEntity>();
      if (monsterWeaponEntity && monsterWeaponEntity.anyAction)
      {
        var contactPoint = other.ClosestPointOnBounds(transform.position);
        var dir = player.transform.position - contactPoint;
        dir.Normalize();
        player.OnFendingOff(monsterWeaponEntity.knockbackForce, dir, contactPoint);
        _slowMotionMonitor.Freeze(.08f, .08f);
        return;
      }
    }
  }
}

public struct KatanaSlashJson
{
  public int slashQueueIndex;
}