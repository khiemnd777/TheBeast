using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
	public float value;
	public float maxValue;
	public float restoreValue;
	public float restoreSeconds;

	float _consumingTime;
	float _restoreDeltaTime;
	bool _isExceeded;
	bool _isConsuming;

	public bool isExceeded { get { return _isExceeded; } }

	void Update ()
	{
		CheckConsuming ();
		Restore ();
		CheckConsumingAgain ();
	}

	public void Consume (float value, System.Action then)
	{
		if (_isExceeded) return;
		if (value > this.value)
		{
			_isExceeded = true;
			return;
		}
		this.value = this.value <= 0 ? 0 : this.value - value;
		if (this.value > 0)
		{
			if (then != null)
			{
				then ();
			}
		}
		_isExceeded = this.value <= 0;
		_isConsuming = true;
		_consumingTime = 0;
	}

	void Restore ()
	{
		if (_isConsuming) return;
		if (value >= maxValue)
		{
			value = maxValue;
			return;
		}
		_restoreDeltaTime += Time.deltaTime / restoreSeconds;
		if (_restoreDeltaTime >= 1)
		{
			value += restoreValue;
			_restoreDeltaTime = 0;
		}
	}

	void CheckConsuming ()
	{
		if (!_isConsuming) return;
		_consumingTime += Time.deltaTime / .5f;
		if (_consumingTime >= 1f)
		{
			_isConsuming = false;
			_consumingTime = 0f;
		}
	}

	void CheckConsumingAgain ()
	{
		if (!_isExceeded) return;
		_isExceeded = value / maxValue < .3f;
	}
}
