using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (SphereCollider))]
public class BlownBang : MonoBehaviour
{
	float _explosionSize;
	float _hitbackForce;
	float _damage;
	Animator _animator;
	SphereCollider _collider;
	CameraShake _cameraShake;
	SlowMotionMonitor _slowMotionMonitor;

	float _animLength;
	float _tAnimLength;

	void Awake ()
	{
		_animator = GetComponent<Animator> ();
		_collider = GetComponent<SphereCollider> ();
		_cameraShake = FindObjectOfType<CameraShake> ();
		_slowMotionMonitor = FindObjectOfType<SlowMotionMonitor> ();
	}

	void Start ()
	{
		_collider.isTrigger = true;
		_animLength = _animator.GetCurrentAnimatorStateInfo (0).length;
	}

	void Update ()
	{
		_tAnimLength += Time.deltaTime / _animLength;
		if (_tAnimLength >= 1f)
		{
			Destroy (gameObject);
		}
	}

	public void Trigger (float explosionSize, float damage, float hitbackForce)
	{
		_hitbackForce = hitbackForce;
		_damage = damage;
		StartCoroutine (Boomb (explosionSize));
	}

	IEnumerator Boomb (float explosionSize)
	{
		_collider.radius = explosionSize;
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / _animLength;
			yield return null;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (!other) return;
		var hitMonster = other.GetComponent<Monster> ();
		if (hitMonster)
		{
			var contactPoint = other.ClosestPointOnBounds (transform.position);
			var dir = contactPoint - other.transform.position;
			dir.Normalize ();
			hitMonster.OnHit (transform, _hitbackForce, dir, contactPoint);
			_cameraShake.Shake (.25f, .25f);
			_slowMotionMonitor.Freeze (.5f, .25f);
		}
		var hitPlayer = other.GetComponent<Player2> ();
		if (hitPlayer)
		{
			var contactPoint = other.ClosestPointOnBounds (transform.position);
			var dir = contactPoint - other.transform.position;
			dir.Normalize ();
			hitPlayer.OnHit (_damage, _hitbackForce, dir, contactPoint);
			_cameraShake.Shake (.25f, .25f);
			_slowMotionMonitor.Freeze (.75f, .35f);
		}
	}
}
