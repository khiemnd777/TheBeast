using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedFoot : MonoBehaviour
{
	public int capacity;
	public float speed;
	public float lifetime;
	[SerializeField]
	EchoEffect _prefab;

	public void Launch (Vector3 position)
	{
		// Echo fx
		var deltaAngle = 360f / capacity;
		for (var i = 0; i < capacity; i++)
		{
			var angle = i * deltaAngle;
			var euler = Quaternion.Euler (0f, angle, 0f);
			var beam = Instantiate<EchoEffect> (_prefab, position, euler);
			beam.speed = speed;
			beam.lifetime = lifetime;		
		}
	}
}
