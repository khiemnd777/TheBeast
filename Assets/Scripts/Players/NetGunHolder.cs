using System.Collections;
using Net;
using UnityEngine;

public class NetGunHolder : MonoBehaviour
{
  public HolderSide holderSide = HolderSide.Right;
  public NetGun gun;

  [SerializeField]
  NetHand _hand;

  [SerializeField]
  Player _player;

  [SerializeField]
  NetIdentity _netIdentity;

  NetGun _heldGun;
  Vector3 _beginPosition;

  public void RotateTowards(Quaternion rotation)
  {
    var gunHolderTransform = this.transform;
    gunHolderTransform.rotation = Quaternion.RotateTowards(gunHolderTransform.rotation, rotation, Time.deltaTime * 630f);
  }

  public void KeepInCover()
  {
    if (_heldGun != null && _heldGun is Object && !_heldGun.Equals(null))
    {
      _heldGun.KeepInCover();
    }
  }

  static object holdGunObjectLock = new object();
  public void TakeUpArm()
  {
    _beginPosition = transform.localPosition;
    if (gun != null && gun is Object && !gun.Equals(null))
    {
      lock (holdGunObjectLock)
      {
        if (!_heldGun)
        {
          _heldGun = Instantiate<NetGun>(gun, transform.position, transform.rotation, transform);
        }
      }
      _heldGun.SetHolderSide(holderSide);
      _heldGun.SetPlayer(_player);
      _heldGun.SetGunWeightToPlayer(_player);
      _heldGun.SetNetIdentity(_netIdentity);
      if (_netIdentity.isLocal)
      {
        _heldGun.TakeUpArm();
        _heldGun.OnAfterTakenUpArm();
        _heldGun.OnProjectileLaunched += OnProjectileLaunched;
        _hand.maximumRange = _heldGun.gunHandType == GunHandType.OneHand ? 1.4f : .8f;
      }
    }
  }

  public void BeforeHoldTrigger()
  {
    StopCoroutine(TakeArmBackToBeginPosition());
    transform.localPosition = _beginPosition;
  }

  public void HoldTrigger(Vector3 dotSightPoint)
  {
    if (_heldGun != null && _heldGun is Object && !_heldGun.Equals(null))
    {
      if (_heldGun.locker.IsLocked()) return;
      _heldGun.HoldTrigger(dotSightPoint);
    }
  }

  public void ReleaseTrigger()
  {
    if (_heldGun != null && _heldGun is Object && !_heldGun.Equals(null))
    {
      _heldGun.ReleaseTrigger();
    }
  }

  void OnProjectileLaunched()
  {
    // knock arm back
    StartCoroutine(KnockArmBack());
  }

  IEnumerator KnockArmBack()
  {
    var expectedKnockbackX = _beginPosition.x - _heldGun.knockbackIndex;
    var t = 0f;
    while (t <= 1f)
    {
      t += Time.deltaTime / .125f;
      var knockbackX = Mathf.Lerp(_beginPosition.x, expectedKnockbackX, t);
      var knockbackPosition = new Vector3(knockbackX, _beginPosition.y, _beginPosition.z);
      transform.localPosition = knockbackPosition;
      yield return null;
    }
    StartCoroutine(TakeArmBackToBeginPosition());
  }

  IEnumerator TakeArmBackToBeginPosition()
  {
    var currentPosition = transform.localPosition;
    var t = 0f;
    while (t <= 1f)
    {
      t += Time.deltaTime / .08f;
      transform.localPosition = Vector3.Lerp(currentPosition, _beginPosition, t);
      yield return null;
    }
  }
}
