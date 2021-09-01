using System.Collections;
using Net;
using UnityEngine;

public class NetMeleeHolder : MonoBehaviour
{
  public NetMelee melee;

  [System.NonSerialized]
  public NetMelee heldMelee;

  public float delay;

  [SerializeField]
  NetHand _hand;

  [SerializeField]
  NetIdentity _netIdentity;

  [SerializeField]
  Player _player;

  Vector3 _beginPosition;
  Animator _handAnimator;
  bool _isHoldingOn;

  void Awake()
  {
    _handAnimator = _hand.GetComponent<Animator>();
  }

  public void KeepInCover()
  {
    if (heldMelee != null && heldMelee is Object && !heldMelee.Equals(null))
    {
      heldMelee.KeepInCover();
    }
  }
  object holdMeleeObjectLock = new object();
  public void TakeUpArm()
  {
    if (melee != null && melee is Object && !melee.Equals(null))
    {
      lock (holdMeleeObjectLock)
      {
        if (!heldMelee)
        {
          heldMelee = Instantiate<NetMelee>(melee, transform.position, transform.rotation, transform);
          heldMelee.SetPlayer(_player);
          heldMelee.SetNetIdentity(_netIdentity);
          heldMelee.TakeUpArm(this, _hand, _handAnimator);
          heldMelee.OnPostInstantiated();
          _hand.maximumRange = 1.4f;
        }
      }
      if (_handAnimator != null && _handAnimator is Object && !_handAnimator.Equals(null))
      {
        _handAnimator.runtimeAnimatorController = heldMelee.meleeAnimatorController;
      }
    }
  }

  public void HoldTrigger()
  {
    if (heldMelee != null && heldMelee is Object && !heldMelee.Equals(null))
    {
      if (_isHoldingOn) return;
      if (gameObject.activeInHierarchy)
      {
        StartCoroutine(OnHoldingTrigger());
      }
    }
  }

  IEnumerator OnHoldingTrigger()
  {
    _isHoldingOn = true;
    yield return StartCoroutine(heldMelee.HoldTrigger());
    // yield return StartCoroutine (WaitingForNextMeleeBeOnTrigger ());
    _isHoldingOn = false;
  }

  IEnumerator WaitingForNextMeleeBeOnTrigger()
  {
    var t = 0f;
    while (t <= 1f)
    {
      t += Time.deltaTime / delay;
      yield return null;
    }
  }
}
