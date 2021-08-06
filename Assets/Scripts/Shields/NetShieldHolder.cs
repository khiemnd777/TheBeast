using Net;
using UnityEngine;

public class NetShieldHolder : MonoBehaviour
{
  public NetShield shield;

  [SerializeField]
  NetHand _hand;

  [SerializeField]
  Player _player;

  [SerializeField]
  NetIdentity _netIdentity;

  NetShield _heldShield;

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
        _heldShield = Instantiate<NetShield>(shield, transform.position, transform.rotation, transform);
        _heldShield.SetPlayer(_player);
        _heldShield.SetNetIdentity(_netIdentity);
      }
    }
  }
}
