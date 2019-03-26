using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaView : MonoBehaviour
{
	public Text text;
	public float refreshRateTime = .25f;
	Stamina _stamina;
	float _t;

	void Awake ()
	{
		_stamina = FindObjectOfType<Stamina> ();
	}

	void Update ()
	{
		_t += Time.deltaTime / refreshRateTime;
		if (_t >= 1)
		{
			text.text = Mathf.RoundToInt (_stamina.value).ToString ();
			_t = 0;
		}
	}
}
