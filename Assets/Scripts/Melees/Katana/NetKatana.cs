using System.Collections;
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
    player.locker.RegisterLock("Kanata");
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
    base.player.locker.Lock("Kanata");
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
    base.player.locker.Unlock("Kanata");
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
          if (dataJson.slashQueueIndex < slashQueue.Count)
          {
            playerAnimator = player.animator;
            playerAnimator.runtimeAnimatorController = meleeAnimatorController;
            _currentSlashAnim = slashQueue[dataJson.slashQueueIndex];
            playerAnimator.Play(_currentSlashAnim.name, 0);
          }
        }
        break;
      default:
        break;
    }
  }

  public override void KeepInCover()
  {
    if (!netIdentity.isLocal)
    {
      Debug.Log("Katana is keeping in the cover");
    }
    if (!netIdentity.isLocal)
    {
      Debug.Log($"Katana's {playerAnimator}");
    }
    playerAnimator.enabled = false;
    if (!netIdentity.isLocal)
    {
      Debug.Log("1");
    }
    hand.enabled = true;
    if (!netIdentity.isLocal)
    {
      Debug.Log("2");
    }
    anyAction = false;
    if (!netIdentity.isLocal)
    {
      Debug.Log("3");
    }
    _trail.enabled = false;
    if (!netIdentity.isLocal)
    {
      Debug.Log("4");
    }
    if (!netIdentity.isLocal)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    if (!netIdentity.isLocal)
    {
      Debug.Log("5");
    }
    if (!netIdentity.isLocal)
    {
      Debug.Log("Katana is kept in the cover!");
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