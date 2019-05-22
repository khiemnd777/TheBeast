using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterZeroFangBomb : MonsterSkill
{
	public SpriteRenderer fangBomb;
	public float minBombSize;
	public float maxBombSize;
	public float bombFullSize;
	public float bombSpeed;
	public float bombDestroyTime;
	public Transform projectile;
	public float smoothSpeed;
	public Transform bubblePrefab;
	public float bubbleSpeed;
	public float bubbleMaxDistance;
	public float loadingBubbleTime;
	public float creatingBubbleTime;
	public float minBubbleSize;
	public float maxBubbleSize;

	ReversedDamage _reversedDamage;
	float _tBecomeFull;
	float _tDestroy;
	Vector3 _directedMoving;
	bool _firstFrameOfMoving = true;
	float _bombSize;

	void Awake ()
	{
		_reversedDamage = GetComponent<ReversedDamage> ();
		_reversedDamage.speed = bombSpeed;
	}

	void Start ()
	{
		fangBomb.transform.localScale = Vector3.zero;
		StartCoroutine (LoadingBubbles ());
		StartCoroutine (BombSizeUp ());
	}

	IEnumerator BombSizeUp ()
	{
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / (loadingBubbleTime * .75f);
			_bombSize = Mathf.Lerp (0, bombFullSize, t);
			yield return null;
		}
	}

	void Update ()
	{
		var r = Random.Range (_bombSize - .15f, _bombSize + .15f);
		fangBomb.transform.localScale = Vector3.one * r;
		_tBecomeFull += Time.deltaTime / loadingBubbleTime;
		if (_tBecomeFull <= 1f)
		{
			var smoothedPos = Vector3.Lerp (transform.position, projectile.position, smoothSpeed);
			transform.position = smoothedPos;
		}
		else
		{
			launched = true;
			_tDestroy += Time.deltaTime / bombDestroyTime;
			if (_tDestroy <= 1f)
			{
				if (_firstFrameOfMoving)
				{
					_directedMoving = projectile.rotation * Vector3.down;
					_firstFrameOfMoving = false;
				}
				if (!_reversedDamage.reversed)
				{
					transform.Translate (_directedMoving * Time.deltaTime * bombSpeed);
				}
				else
				{
					transform.Translate (-_directedMoving * Time.deltaTime * _reversedDamage.speed);
				}
				_tDestroy += Time.deltaTime / bombDestroyTime;
				if (_tDestroy >= 1f)
				{

				}
			}
			else
			{
				Destroy (gameObject);
			}
		}
	}

	IEnumerator LoadingBubbles ()
	{
		var tLoading = 0f;
		var tCreatingBubble = 0f;
		while (tLoading <= 1f)
		{
			tLoading += Time.deltaTime / loadingBubbleTime;
			if (tCreatingBubble <= 1f)
			{
				tCreatingBubble += Time.deltaTime / creatingBubbleTime;
			}
			else
			{
				var bubblePos = Random.insideUnitSphere * bubbleMaxDistance + projectile.position;
				var bubbleDir = bubblePos - projectile.position;
				bubbleDir.Normalize ();
				var bubbleRot = Utilities.RotateByNormal (bubbleDir, Vector3.up);
				var bubbleIns = Instantiate<Transform> (bubblePrefab, bubblePos, bubbleRot);
				bubbleIns.localScale = Vector3.one * Random.Range (minBubbleSize, maxBubbleSize);
				StartCoroutine (BubbleMoving (bubbleIns));
			}
			yield return null;
		}
	}

	IEnumerator BubbleMoving (Transform bubble)
	{
		var t = 0f;
		var startPos = bubble.position;
		while (t <= 1f)
		{
			t += Time.deltaTime * bubbleSpeed;
			bubble.position = Vector3.Lerp (startPos, projectile.position, t);
			yield return null;
		}
		Destroy (bubble.gameObject);
	}

	void OnTriggerEnter (Collider other)
	{
		if (_reversedDamage.reversed && other.gameObject.layer == LayerMask.NameToLayer ("Enemy"))
		{
			Destroy (gameObject);
			return;
		}
		if (other.gameObject.layer != LayerMask.NameToLayer ("Enemy") &&
			other.gameObject.layer != LayerMask.NameToLayer ("Reversed Damage"))
		{
			Destroy (gameObject);
		}
	}
}
