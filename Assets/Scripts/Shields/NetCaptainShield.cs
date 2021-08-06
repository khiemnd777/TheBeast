using System.Collections;
using UnityEngine;

public class NetCaptainShield : NetShield
{
  [SerializeField]
  AnimationClip _shieldCommonAnim;
  [SerializeField]
  AnimationClip _shieldReversionAnim;
  bool _isReversing;

  public override void Start()
  {
    player.locker.RegisterLock("ReverseCaptainShield");
  }

  public override void TakeShieldUpAsCover()
  {
    if (_isReversing) return;
    base.TakeShieldUpAsCover();
    player.animator.Play(_shieldCommonAnim.name, 0);
  }

  public override void TakeShieldAsReverse()
  {
    player.locker.Lock("ReverseCaptainShield");
    base.TakeShieldAsReverse();
    _isReversing = true;
    player.animator.Play(_shieldReversionAnim.name, 0);
    StartCoroutine(ReversingCountDown());
  }

  IEnumerator ReversingCountDown()
  {
    yield return new WaitForSeconds(_shieldReversionAnim.length);
    _isReversing = false;
    player.locker.Unlock("ReverseCaptainShield");
  }

  public override void TakeShieldDown()
  {
    if (player.weaponController.meleeHolderController.rightMeleeHolder.heldMelee)
    {
      player.animator.runtimeAnimatorController = player.weaponController.meleeHolderController.rightMeleeHolder.heldMelee.meleeAnimatorController;
    }
    base.TakeShieldDown();
  }
}
