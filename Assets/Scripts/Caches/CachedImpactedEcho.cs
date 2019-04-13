using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedImpactedEcho : MonoBehaviour
{
	public int capacity = 360;
	List<ImpactedEcho> _impactedEchos = new List<ImpactedEcho> ();

	public List<ImpactedEcho> impactedEchos
	{
		get
		{
			return _impactedEchos;
		}
	}

	[SerializeField]
	ImpactedEcho _prefab;

	void Awake ()
	{
		Init (capacity);
	}

	void Init (int capacity)
	{
		for (var i = 0; i < capacity; i++)
		{
			var ins = Instantiate<ImpactedEcho> (_prefab, transform.position, Quaternion.identity);
			_impactedEchos.Add (ins);
		}
	}

	public void Use (RaycastHit hit, LayerMask layerMask)
	{
		var amountCount = 1;
		// var targetNormal = hit.normal;
		// var impactedEchoAngle = 360f - 180f - Mathf.Atan2 (targetNormal.z, targetNormal.x) * Mathf.Rad2Deg;
		// var impactedWaveRot = Quaternion.Euler (0f, impactedEchoAngle, 0f);
		for (var i = 0; i < capacity; i++)
		{
			if (amountCount == 0) break;
			var impactedWave = _impactedEchos[i];
			if (!impactedWave.free) continue;
			// impactedWave.transform.rotation = impactedWaveRot;
			// impactedWave.transform.position = hit.point;
			impactedWave.hit = hit;
			impactedWave.layerMask = layerMask;
			impactedWave.impactedObject = hit.transform;
			impactedWave.impactedPoint = hit.point;
			impactedWave.Use ();
			--amountCount;
		}
	}
}
