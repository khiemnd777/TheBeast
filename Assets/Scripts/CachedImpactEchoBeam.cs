using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedImpactEchoBeam : MonoBehaviour
{
	public int capacity = 360;
	List<ImpactEchoBeam> _impactEchoBeams = new List<ImpactEchoBeam> ();

	public List<ImpactEchoBeam> impactEchoBeams
	{
		get
		{
			return _impactEchoBeams;
		}
	}

	[SerializeField]
	ImpactEchoBeam _impactEchoBeamPrefab;

	void Awake ()
	{
		Init (capacity);
	}

	void Init (int capacity)
	{
		for (var i = 0; i < capacity; i++)
		{
			var impactEchoBeam = Instantiate<ImpactEchoBeam> (_impactEchoBeamPrefab, transform.position, Quaternion.identity);
			_impactEchoBeams.Add (impactEchoBeam);
		}
	}

	void InstantiateImpactEcho (Vector3 direction, Vector2 normal)
	{
		var perpendicular = new Vector2 (normal.y, -normal.x);
		var fromAngle = Mathf.Atan2 (perpendicular.y, perpendicular.x) * Mathf.Rad2Deg;
		var amount = 1f;
		var deltaAngle = 180f / amount;
		for (var i = 0; i <= amount; i++)
		{
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, 0f, angle);
			Instantiate<ImpactEchoBeam> (_impactEchoBeamPrefab, transform.position, euler);
		}
	}

	public void Use (Vector2 pos, Vector3 direction, Vector2 normal)
	{
		var amount = 1f;
		var deltaAngle = 180f / amount;
		var amountCount = amount + 1;
		var perpendicular = new Vector2 (normal.y, -normal.x);
		var fromAngle = Mathf.Atan2 (perpendicular.y, perpendicular.x) * Mathf.Rad2Deg;
		for (var i = 0; i < capacity; i++)
		{
			if (amountCount == 0) break;
			var impactEchoBeam = _impactEchoBeams[i];
			if (!impactEchoBeam.free) continue;
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, 0f, angle);
			impactEchoBeam.transform.rotation = euler;
			impactEchoBeam.Use (pos);
			--amountCount;
		}
	}
}
