using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedImpactedWave : MonoBehaviour
{
	public int capacity = 360;
	List<ImpactedWave> _impactedWaves = new List<ImpactedWave> ();

	public List<ImpactedWave> impactedWaves
	{
		get
		{
			return _impactedWaves;
		}
	}

	[SerializeField]
	ImpactedWave _prefab;

	void Awake ()
	{
		Init (capacity);
	}

	void Init (int capacity)
	{
		for (var i = 0; i < capacity; i++)
		{
			var ins = Instantiate<ImpactedWave> (_prefab, transform.position, Quaternion.identity);
			_impactedWaves.Add (ins);
		}
	}

	public void Use (RaycastHit2D hit, LayerMask layerMask)
	{
		var amountCount = 1;
		var targetNormal = hit.normal;
		var impactedWaveAngle = 180f + Mathf.Atan2 (targetNormal.y, targetNormal.x) * Mathf.Rad2Deg;
		var impactedWaveRot = Quaternion.Euler (0, 0, impactedWaveAngle);
		for (var i = 0; i < capacity; i++)
		{
			if (amountCount == 0) break;
			var impactedWave = _impactedWaves[i];
			if (!impactedWave.free) continue;
			impactedWave.capacity = 4f;
			impactedWave.transform.rotation = impactedWaveRot;
			impactedWave.transform.position = hit.point;
			impactedWave.layerMask = layerMask;
			impactedWave.impactedObject = hit.transform;
			impactedWave.impactedPoint = hit.point;
			impactedWave.Use ();
			--amountCount;
		}
	}
}
