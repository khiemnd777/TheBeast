using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSView : MonoBehaviour
{
	public Text text;
	float _t;

	void Update ()
	{
		_t += Time.deltaTime / .5f;
		if (_t >= 1)
		{
			var fps = 1 / Time.deltaTime;
			text.text = Mathf.RoundToInt (fps) + "fps";
			_t = 0;
		}
	}
}
