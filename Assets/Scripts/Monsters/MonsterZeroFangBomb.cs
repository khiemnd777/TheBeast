using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterZeroFangBomb : MonoBehaviour
{
	public SpriteRenderer fangBomb;
	public float minBombSize;
	public float maxBombSize;
	public float bombSpeed;
	public float bombDestroyTime;
	public Transform projectile;
	public float smoothSpeed;
	public Transform bubblePrefab;
	public float bubbleSpeed;
	public float loadingBubbleTime;
	public float creatingBubbleTime;
	public float minBubbleSize;
	public float maxBubbleSize;

	float _tBecomeFull;
	float _tDestroy;
	Vector3 _directedMoving;
	bool _firstFrameOfMoving = true;

	void Start ()
	{
		StartCoroutine (LoadingBubbles ());
	}

	void Update ()
	{
		var r = Random.Range (minBombSize, maxBombSize);
		fangBomb.transform.localScale = Vector3.one * r;
		_tBecomeFull += Time.deltaTime / loadingBubbleTime;
		if (_tBecomeFull <= 1f)
		{
			var smoothedPos = Vector3.Lerp (transform.position, projectile.position, smoothSpeed);
			transform.position = smoothedPos;
		}
		else
		{
			_tDestroy += Time.deltaTime / bombDestroyTime;
			if (_tDestroy <= 1f)
			{
				if (_firstFrameOfMoving)
				{
					_directedMoving = projectile.rotation * Vector3.down;
					_firstFrameOfMoving = false;
				}
				transform.Translate (_directedMoving * Time.deltaTime * bombSpeed);
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
				var bubblePos = Random.insideUnitSphere * 1.865f + projectile.position;
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
}
