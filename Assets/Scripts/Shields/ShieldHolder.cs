using UnityEngine;

public class ShieldHolder : MonoBehaviour
{
  public Shield shield;
  [SerializeField]
  Hand _hand;
  Shield _heldShield;

  public void TakeShieldDown()
  {
    ShieldInHand();
    if (_heldShield.IsNotNull())
    {
      _heldShield.TakeShieldDown();
    }
  }

  public void TakeShieldUpAsCover()
  {
    ShieldInHand();
    if (_heldShield.IsNotNull())
    {
      _heldShield.TakeShieldUpAsCover();
    }
  }

  public void TakeShieldAsReverse()
  {
    ShieldInHand();
    if (_heldShield.IsNotNull())
    {
      _heldShield.TakeShieldAsReverse();
    }
  }

  void ShieldInHand()
  {
    if (shield.IsNotNull())
    {
      if (!_heldShield)
      {
        _heldShield = Instantiate<Shield>(shield, transform.position, transform.rotation, transform);
      }
    }
  }
}
