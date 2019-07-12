using UnityEngine;

public class ShieldHolderController : MonoBehaviour
{
	public ShieldHolder shieldHolder;
	public float timeHoldShieldTrigger;

	DotSight _dotSight;
	bool _isLeft;
	bool _isKeyHoldingDown;

	void Awake ()
	{
		_dotSight = FindObjectOfType<DotSight> ();
	}

	public void DoUpdating ()
	{
		if (Input.GetKeyDown (KeyCode.LeftShift))
		{
			_isKeyHoldingDown = true;
		}
		if (Input.GetKeyUp (KeyCode.LeftShift))
		{
			_isKeyHoldingDown = false;
			ReleaseTriggers ();
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
		// if (!_isKeyHoldingDown) return;
		// {
		// 	// HoldTrigger (shieldHolder);
		// }
	}

	void ReleaseTriggers ()
	{
		TakeShieldDown ();
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
}
