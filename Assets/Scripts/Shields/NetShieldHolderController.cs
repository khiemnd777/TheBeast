using Net;
using UnityEngine;

public class NetShieldHolderController : MonoBehaviour
{
  [SerializeField]
  NetIdentity _netIdentity;
  public float timeHoldShieldTrigger;
  public NetShieldHolder shieldHolder;
  public NetWeaponController weaponController;

  bool _isLeft;
  bool _isKeyHoldingDown;
  bool _isReversing;

  public void DoUpdating()
  {
    if (_netIdentity.isLocal)
    {
      if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        _isKeyHoldingDown = true;
        TakeShieldUpAsCover();
      }
      if (Input.GetKeyUp(KeyCode.LeftShift))
      {
        _isKeyHoldingDown = false;
        TakeShieldDown();
      }
      HoldTriggers();
    }
  }

  public void TakeShieldDown()
  {
    TakeShieldDown(shieldHolder);
  }

  public void TakeShieldUpAsCover()
  {
    TakeShieldUpAsCover(shieldHolder);
  }

  void HoldTriggers()
  {
    if (!_isKeyHoldingDown) return;
    if (Input.GetMouseButtonDown(1))
    {
      ReverseTrigger();
      return;
    }
    if ((weaponController?.meleeHolderController?.rightMeleeHolder?.heldMelee?.anyAction).GetValueOrDefault()) return;
    TakeShieldUpAsCover();
  }

  void ReverseTrigger()
  {
    if (weaponController.meleeHolderController.rightMeleeHolder.heldMelee.anyAction) return;
    TakeShieldAsReverse(shieldHolder);
  }

  void TakeShieldDown(NetShieldHolder shieldHolder)
  {
    if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals(null))
    {
      shieldHolder.TakeShieldDown();
    }
  }

  void TakeShieldUpAsCover(NetShieldHolder shieldHolder)
  {
    if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals(null))
    {
      shieldHolder.TakeShieldUpAsCover();
    }
  }

  void TakeShieldAsReverse(NetShieldHolder shieldHolder)
  {
    if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals(null))
    {
      shieldHolder.TakeShieldAsReverse();
    }
  }
}
