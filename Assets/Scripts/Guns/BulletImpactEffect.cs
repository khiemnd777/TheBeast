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

	public void Use (Vector3 impactPoint, Vector3 normal)
	{
		var capacity = 18;
		var deltaAngle = 360f / capacity;
		var perpendicular = new Vector3 (normal.z, 0f, -normal.x);
		var fromAngle = 180f - Mathf.Atan2 (perpendicular.z, perpendicular.x) * Mathf.Rad2Deg;
		for (var i = 0; i <= capacity; i++)
		{
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, angle, 0f);
			var beam = Instantiate<ImpactEffect> (_impactEffectPrefab, impactPoint, euler);
			beam.speed = Random.Range (minSpeed, maxSpeed);
			beam.lifetime = lifetime;
		}
	}

	public void Use (Vector3 impactPoint, Vector3 normal, Transform targetHit)
	{
		var capacity = 18;
		var deltaAngle = 360f / capacity;
		var perpendicular = new Vector3 (normal.z, 0f, -normal.x);
		var fromAngle = 180f - Mathf.Atan2 (perpendicular.z, perpendicular.x) * Mathf.Rad2Deg;
		for (var i = 0; i <= capacity; i++)
		{
			var angle = fromAngle + i * deltaAngle;
			var euler = Quaternion.Euler (0f, angle, 0f);
			var beam = Instantiate<ImpactEffect> (_impactEffectPrefab, impactPoint, euler, targetHit);
			beam.speed = Random.Range (minSpeed, maxSpeed);
			beam.lifetime = lifetime;
		}
	}
}
