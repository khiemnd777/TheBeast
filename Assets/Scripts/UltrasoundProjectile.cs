using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltrasoundProjectile : MonoBehaviour
{
	public float timeBetweenLaunch;
	[SerializeField]
	Transform _projectile;
	[SerializeField]
	Ultrasound _ultrasoundPrefab;
	float _tbl = 1f;

	void Update ()
	{
		if(_tbl < 1f){
			_tbl += Time.deltaTime / timeBetweenLaunch;
		}
		if (_tbl >= 1f)
		{
			if (Input.GetMouseButtonUp (1))
			{
				Instantiate<Ultrasound> (_ultrasoundPrefab, _projectile.position, _projectile.rotation);
				_tbl = 0f;
			}
		}
	}
}
