using UnityEngine;

public class ShieldHolderController : MonoBehaviour
{
	public ShieldHolder shieldHolder;
	public float timeHoldShieldTrigger;
	public WeaponController weaponController;

	DotSight _dotSight;
	bool _isLeft;
	bool _isKeyHoldingDown;
	bool _isReversing;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	public void DoUpdating ()
	{
		if (Input.GetKeyDown (KeyCode.LeftShift))
		{
			_isKeyHoldingDown = true;
			// TakeShieldUpAsCover ();
		}
		if (Input.GetKeyUp (KeyCode.LeftShift))
		{
			// Debug.Log(1);
			_isKeyHoldingDown = false;
			// ReleaseTriggers ();
			TakeShieldDown ();
		}
		HoldTriggers ();
	}

	public void TakeShieldDown ()
	{
		TakeShieldDown (shieldHolder);
	}

	public void TakeShieldUpAsCover ()
	{
		TakeShieldUpAsCover (shieldHolder);
	}

	void HoldTriggers ()
	{
		if (!_isKeyHoldingDown) return;
		if (Input.GetMouseButtonDown (1))
		{
			ReverseTrigger ();
			return;
		}
		if (weaponController.meleeHolderController.rightMeleeHolder.heldMelee.anyAction) return;
		TakeShieldUpAsCover ();
	}

	void ReleaseTriggers ()
	{
		TakeShieldDown ();
	}

	void ReverseTrigger ()
	{
		if (weaponController.meleeHolderController.rightMeleeHolder.heldMelee.anyAction) return;
		TakeShieldAsReverse (shieldHolder);
	}

	void TakeShieldDown (ShieldHolder shieldHolder)
	{
		if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals (null))
		{
			shieldHolder.TakeShieldDown ();
		}
	}

	void TakeShieldUpAsCover (ShieldHolder shieldHolder)
	{
		if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals (null))
		{
			shieldHolder.TakeShieldUpAsCover ();
		}
	}

	void TakeShieldAsReverse (ShieldHolder shieldHolder)
	{
		if (shieldHolder != null && shieldHolder is Object && !shieldHolder.Equals (null))
		{
			shieldHolder.TakeShieldAsReverse ();
		}
	}
}
