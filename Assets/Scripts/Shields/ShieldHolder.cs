using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHolder : MonoBehaviour
{
	public Shield shield;
	[SerializeField]
	Hand _hand;
	Shield _heldShield;
	Vector3 _beginPosition;

	public void TakeShieldDown ()
	{
		if (_heldShield != null && _heldShield is Object && !_heldShield.Equals (null))
		{
			_heldShield.TakeShieldDown ();
		}
	}

	public void TakeShieldUpAsCover ()
	{
		_beginPosition = transform.localPosition;
		if (shield != null && shield is Object && !shield.Equals (null))
		{
			if (!_heldShield)
			{
				_heldShield = Instantiate<Shield> (shield, transform.position, transform.rotation, transform);
			}
			_heldShield.TakeShieldUpAsCover ();
		}
	}
}
