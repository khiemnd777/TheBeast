using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Animator))]
public class BlownBang : MonoBehaviour
{
	public Animator animator;
	float _animLength;
	float _tAnimLength;

	void Start ()
	{
		_animLength = animator.GetCurrentAnimatorStateInfo (0).length;
	}

	void Update ()
	{
		_tAnimLength += Time.deltaTime / _animLength;
		if (_tAnimLength >= 1f)
		{
			Destroy (gameObject);
		}
	}
}
