using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHolder : MonoBehaviour
{
	public Shield shield;
	[SerializeField]
	Hand _hand;
	Shield _heldShield;

	public void TakeShieldDown ()
	{
		ShieldInHand ();
		if (_heldShield != null && _heldShield is Object && !_heldShield.Equals (null))
		{
			_heldShield.TakeShieldDown ();
		}
	}

	public void TakeShieldUpAsCover ()
	{
		ShieldInHand ();
		_heldShield.TakeShieldUpAsCover ();
	}

	public void TakeShieldAsReverse ()
	{
		ShieldInHand ();
		_heldShield.TakeShieldAsReverse ();
	}

	void ShieldInHand ()
	{
		if (shield != null && shield is Object && !shield.Equals (null))
		{
			if (!_heldShield)
			{
				_heldShield = Instantiate<Shield> (shield, transform.position, transform.rotation, transform);
			}
		}
	}
}
