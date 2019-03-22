using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltrasoundSense : MonoBehaviour
{
	public float consumedEnergy;
	public float echoLifetime;
	[SerializeField]
	Echo _echoPrefab;

	Stamina _stamina;

	void Awake ()
	{
		_stamina = FindObjectOfType<Stamina> ();
	}

	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.F))
		{
			if (_stamina.isExceeded) return;
			_stamina.Consume (consumedEnergy, () =>
			{
				var echo = Instantiate<Echo> (_echoPrefab, transform.position, Quaternion.identity);
				echo.lifetime = echoLifetime;
				Destroy (echo.gameObject, echo.lifetime + .1f);
			});
		}
	}
}
