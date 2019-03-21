using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpactEffect : MonoBehaviour
{
	[SerializeField]
	ImpactEffect _impactEffectPrefab;
	public float lifetime;
	public float maxSpeed;
	public float minSpeed = 2;

	public void Use (Vector2 impactPoint, Vector3 normal)
	{
		var capacity = 9;
		var deltaAngle = 180f / capacity;
		var perpendicular = new Vector2 (normal.y, -normal.x);
		var fromAngle = Mathf.Atan2 (perpendicular.y, perpendicular.x) * Mathf.Rad2Deg;
		for (var i = 0; i <= capacity; i++)
		{
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, 0f, angle);
			var beam = Instantiate<ImpactEffect> (_impactEffectPrefab, impactPoint, euler);
			beam.speed = Random.Range (minSpeed, maxSpeed); // i == 0 || i == capacity ? 4 : 3;
			beam.lifetime = lifetime; //.125f;
		}
	}
}
