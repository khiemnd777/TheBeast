using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedEchoBeam : MonoBehaviour
{
	public int capacity = 720;
	List<Beam> _echoBeams = new List<Beam> ();
	public List<Beam> eachBeams
	{
		get
		{
			return _echoBeams;
		}
	}

	[SerializeField]
	Beam _echoBeamPrefab;

	void Awake ()
	{
		Init (capacity);
	}

	void Init (int capacity)
	{
		for (var i = 0; i < capacity; i++)
		{
			var echoBeam = Instantiate<Beam> (_echoBeamPrefab, transform.position, Quaternion.identity);
			_echoBeams.Add (echoBeam);
		}
	}

	public List<Beam> GetEchoBeam (int amount)
	{
		var result = new List<Beam> ();
		var amountCount = amount;
		for (var i = 0; i < capacity; i++)
		{
			if (amountCount == 0) break;
			var echoBeam = _echoBeams[i];
			if (!echoBeam.free) continue;
			result.Add (echoBeam);
			--amountCount;
		}
		if (amountCount > 0)
		{
			var echoBeam = Instantiate<Beam> (_echoBeamPrefab, transform.position, Quaternion.identity);
			result.Add (echoBeam);
		}
		return result;
	}

	public void Use (int amount, Vector2 pos, float speed, float raycastDistance, float lifetime)
	{
		var deltaAngle = 360f / amount;
		var amountCount = amount;
		var startAngle = Random.Range (0, 360f);
		for (var i = 0; i < capacity; i++)
		{
			if (amountCount == 0) break;
			var echoBeam = _echoBeams[i];
			if (!echoBeam.free) continue;
			var angle = startAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, 0f, angle);
			echoBeam.transform.rotation = euler;
			echoBeam.speed = speed;
			echoBeam.distance = raycastDistance;
			echoBeam.lifetime = lifetime;
			echoBeam.Use (pos);
			--amountCount;
		}
		if (amountCount > 0)
		{
			for (var i = 0; i < amountCount; i++)
			{
				var angle = startAngle + (amountCount + i) * deltaAngle;
				var euler = Quaternion.Euler (0f, 0f, angle);
				var echoBeam = Instantiate<Beam> (_echoBeamPrefab, pos, euler);
				echoBeam.speed = speed;
				echoBeam.distance = raycastDistance;
				echoBeam.lifetime = lifetime;
				echoBeam.Use (pos);
			}
		}
	}
}
