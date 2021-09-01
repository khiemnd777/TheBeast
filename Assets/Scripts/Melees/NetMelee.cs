using System.Collections;
using System.Collections.Generic;
using Net;
using UnityEngine;

public class NetMelee : MonoBehaviour
{
  public float damage;
  public float range;
  public float size;
  public LayerMask layerMask;

  [Space]
  [SerializeField]
  protected AnimationClip commonStyleAnim;

  [Space]
  public List<MeleeActionQueue> slashQueue2;


  [Space]
  public float freezedTime;
  protected NetHand hand;
  protected Animator playerAnimator;
  public float hitback;
  protected NetMeleeHolder holder;

  public RuntimeAnimatorController meleeAnimatorController;

  [System.NonSerialized]
  public bool anyAction;

  protected Player player;
  protected NetIdentity netIdentity;

  int _slashQueueIndex;
  float _startTriggerTime;
  float _endTriggerTime;
  MeleeActionQueue _currentMeleeActionQueue;

  // public virtual IEnumerator HoldTrigger()
  // {
  //   yield break;
  // }

  public virtual IEnumerator HoldTrigger()
  {
    _startTriggerTime = Time.time;
    var _triggerDistanceTime = _startTriggerTime - _endTriggerTime;
    var resetFirstSlash = _triggerDistanceTime > .3f;
    if (resetFirstSlash)
    {
      _currentMeleeActionQueue = slashQueue2[0];
      _slashQueueIndex = 0;
    }
    else
    {
      ++_slashQueueIndex;
      if (_slashQueueIndex >= slashQueue2.Count)
      {
        _slashQueueIndex = 0;
      }
      _currentMeleeActionQueue = slashQueue2[_slashQueueIndex];
    }
    // Katana trigger to server and another clients.
    netIdentity.EmitMessage("melee_trigger", new MeleeSlashJson
    {
      slashQueueIndex = _slashQueueIndex
    });
    this.player.locker.Lock("MeleeAction");
    playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    OnBeforePlayAnimation();
    anyAction = true;
    hand.enabled = false;

    // Call collider while play the animation
    // StartCoroutine(OnColliding(_currentMeleeActionQueue));

    // Player animation.
    playerAnimator.Play(_currentMeleeActionQueue.animationClip.name, 0);
    yield return new WaitForSeconds(_currentMeleeActionQueue.animationClip.length);

    _endTriggerTime = Time.time;
    anyAction = false;
    hand.enabled = true;
    OnAfterPlayAnimation();
    this.player.locker.Unlock("MeleeAction");
  }

  IEnumerator OnColliding(MeleeActionQueue meleeActionQueue)
  {
    yield return new WaitForFixedUpdate();
    if (player.meleeCollider)
    {
      yield return new WaitForSeconds(meleeActionQueue.animationClip.length * meleeActionQueue.delayRateOnCollision);
      player.meleeCollider.Collide(meleeActionQueue.damage, freezedTime, hitback);
    }
  }

  public virtual void OnBeforePlayAnimation()
  {

  }

  public virtual void OnAfterPlayAnimation()
  {

  }

  public virtual void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    this.hand = hand;
    this.holder = holder;
    if (player.meleeCollider)
    {
      player.meleeCollider.Setup(new MeleeColliderOptions
      {
        range = range,
        size = size,
        layerMask = layerMask
      });
    }
    if (player.animator)
    {
      player.animator.enabled = true;
      playerAnimator = player.animator;
      playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    }
  }

  public virtual void KeepInCover()
  {
    anyAction = false;
    if (!netIdentity.isLocal)
    {
      netIdentity.onMessageReceived -= OnMessageReceived;
    }
    if (playerAnimator) playerAnimator.enabled = false;
    if (hand) hand.enabled = true;
    if (player.meleeCollider)
    {
      player.meleeCollider.Reset();
    }
    Destroy(gameObject);
  }

  public virtual void Awake()
  {

  }

  public virtual void OnPostInstantiated()
  {
    if (!netIdentity.isLocal)
    {
      netIdentity.onMessageReceived += OnMessageReceived;
    }
  }

  void OnMessageReceived(string eventName, string message)
  {
    switch (eventName)
    {
      case "melee_trigger":
        {
          if (!netIdentity.isLocal)
          {
            var dataJson = Utility.Deserialize<MeleeSlashJson>(message);
            anyAction = true;
            hand.enabled = false;
            playerAnimator = player.animator;
            playerAnimator.runtimeAnimatorController = meleeAnimatorController;
            if (gameObject.activeInHierarchy)
            {
              StartCoroutine(OnMeleeTriggerAnim(dataJson.slashQueueIndex));
            }
          }
        }
        break;
      default:
        break;
    }
  }

  IEnumerator OnMeleeTriggerAnim(int slashQueueIndex)
  {
    if (slashQueueIndex < slashQueue2.Count)
    {
      _currentMeleeActionQueue = slashQueue2[slashQueueIndex];

      if (netIdentity.isServer)
      {
        // Call collider while play the animation
        StartCoroutine(OnColliding(_currentMeleeActionQueue));
      }

      // Play animation
      playerAnimator.Play(_currentMeleeActionQueue.animationClip.name, 0);
      yield return new WaitForSeconds(_currentMeleeActionQueue.animationClip.length);
    }
    anyAction = false;
    hand.enabled = true;
  }

  public virtual void Start()
  {
    player.locker.RegisterLock("MeleeAction");
  }

  public virtual void Update()
  {

  }

  public virtual void FixedUpdate()
  {

  }

  public virtual Vector3 GetDirection()
  {
    var direction = Utility.GetDirection(transform, Vector3.back);
    return direction * holder.transform.localScale.z;
  }

  public Vector3 GetNormalDirection()
  {
    var direction = GetDirection();
    direction.Normalize();
    return direction;
  }

  public void SetPlayer(Player player)
  {
    this.player = player;
  }

  public void SetNetIdentity(NetIdentity netIdentity)
  {
    this.netIdentity = netIdentity;
  }
}

[System.Serializable]
public struct MeleeActionQueue
{
  public float damage;

  [Range(0, 1)]
  public float delayRateOnCollision;

  public AnimationClip animationClip;
}

public struct MeleeSlashJson
{
  public int slashQueueIndex;
}