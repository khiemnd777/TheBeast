using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterZeroFangBomb : MonoBehaviour
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
	public BlownBang blownBangPrefab;

	ReversedObject _reversedObject;
	float _tBecomeFull;
	float _tDestroy;
	float _bombSize;
	Vector3 _directedMoving;
	bool _firstFrameOfMoving = true;
	bool _launched;
	List<Transform> _bubbles = new List<Transform> ();

	public void Launch ()
	{
		_launched = true;
	}

	void Awake ()
	{
		_reversedObject = GetComponent<ReversedObject> ();
		_reversedObject.speed = bombSpeed;
		_reversedObject.reversed = false;
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
		if (!_launched)
		{
			var smoothedPos = Vector3.Lerp (transform.position, projectile.position, smoothSpeed);
			transform.position = smoothedPos;
			return;
		}
		if (_tBecomeFull <= 1f) return;
		_tDestroy += Time.deltaTime / bombDestroyTime;
		if (_tDestroy <= 1f)
		{
			if (_firstFrameOfMoving)
			{
				_directedMoving = projectile.rotation * Vector3.down;
				_firstFrameOfMoving = false;
			}
			if (!_reversedObject.reversed)
			{
				transform.Translate (_directedMoving * Time.deltaTime * bombSpeed);
			}
			else
			{
				// transform.Translate (_reversedObject.normal * Time.deltaTime * _reversedObject.speed);
				transform.Translate (-_directedMoving * Time.deltaTime * _reversedObject.speed);
			}
			_tDestroy += Time.deltaTime / bombDestroyTime;
			if (_tDestroy >= 1f)
			{

			}
		}
		else
		{
			BlowBang ();
			Destroy (gameObject);
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
				tCreatingBubble = 0f;
				var bubblePos = Random.insideUnitSphere * bubbleMaxDistance + transform.position;
				var bubbleDir = bubblePos - transform.position;
				bubbleDir.Normalize ();
				var bubbleRot = Utilities.RotateByNormal (bubbleDir, Vector3.up);
				var bubbleIns = Instantiate<Transform> (bubblePrefab, bubblePos, bubbleRot);
				bubbleIns.localScale = Vector3.one * Random.Range (minBubbleSize, maxBubbleSize);
				_bubbles.Add (bubbleIns);
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
			bubble.position = Vector3.Lerp (startPos, transform.position, t);
			yield return null;
		}
		Destroy (bubble.gameObject);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer (LayerConstants.ReversedDamageable))
		{
			return;
		}
		if (_reversedObject.reversed && other.gameObject.layer == LayerMask.NameToLayer ("Enemy"))
		{
			BlowBang ();
			DestroyAll ();
			return;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer ("Player") ||
			other.gameObject.layer == LayerMask.NameToLayer ("Obstacle"))
		{
			BlowBang ();
			DestroyAll ();
		}
	}

	void DestroyAll ()
	{
		foreach (var bubble in _bubbles)
		{
			if (!bubble) continue;
			Destroy (bubble.gameObject);
		}
		Destroy (gameObject);
	}

	void BlowBang ()
	{
		var explosion = Instantiate<BlownBang> (blownBangPrefab, transform.position, Quaternion.identity);
		explosion.Trigger (1.75f, 0f, 10f);
	}
}
