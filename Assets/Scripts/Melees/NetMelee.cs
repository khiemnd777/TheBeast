﻿using System.Collections;
using Net;
using UnityEngine;

public class NetMelee : MonoBehaviour
{
  public float damage;
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

  public virtual IEnumerator HoldTrigger()
  {
    yield break;
  }

  public virtual void TakeUpArm(NetMeleeHolder holder, NetHand hand, Animator handAnimator)
  {
    this.hand = hand;
    this.holder = holder;
    if (player.animator)
    {
      player.animator.enabled = true;
      playerAnimator = player.animator;
      playerAnimator.runtimeAnimatorController = meleeAnimatorController;
    }
  }

  public virtual void KeepInCover()
  {
    Destroy(gameObject);
  }

  public virtual void Awake()
  {

  }

  public virtual void Start()
  {

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
