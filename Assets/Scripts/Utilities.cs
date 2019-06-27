using UnityEngine;

public class Utilities
{
    public static Vector3 CameraInBound (Camera camera, BoxCollider bound, Vector3 position)
    {
        var min = bound.bounds.min;
        var max = bound.bounds.max;
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var x = Mathf.Clamp (position.x, min.x + halfWidth, max.x - halfWidth);
        var z = Mathf.Clamp (position.z, min.z + halfHeight, max.z - halfHeight);
        return new Vector3 (x, position.y, z);
    }

    public static Vector3 CameraInBound2D (Camera camera, BoxCollider2D bound, Vector3 position)
    {
        var min = bound.bounds.min;
        var max = bound.bounds.max;
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var x = Mathf.Clamp (position.x, min.x + halfWidth, max.x - halfWidth);
        var y = Mathf.Clamp (position.y, min.y + halfHeight, max.y - halfHeight);
        return new Vector3 (x, y, position.z);
    }

    public static Vector3 AlterVector3 (Vector3 owner, float x, float y)
    {
        return new Vector3 (x, owner.y, y);
    }

    public static Vector3 HitbackVelocity (Vector3 velocity, Vector3 hitbackNormal, float hitback)
    {
        var hitVel = hitbackNormal * hitback;
        var hitVelLength = hitVel.sqrMagnitude;
        var velLength = velocity.sqrMagnitude;
        var returnedVel = velLength >= hitVelLength ? velocity + hitVel : hitVel;
        return returnedVel;
    }

    public static Quaternion RotateByNormal (Vector3 normal, Vector3 axis)
    {
        var rot = 360f - Mathf.Atan2 (normal.z, normal.x) * Mathf.Rad2Deg;
        var vectRot = axis * rot;
        var qtrn = Quaternion.Euler (vectRot);
        return qtrn;
    }

    public static float DistanceFromTarget (Vector3 targetPosition, Vector3 position)
    {
        return (targetPosition - position).magnitude;
    }

    public static Blood BleedOutAtPoint (Blood bloodPrefab, Vector3 normal, Vector3 bleedPoint)
    {
        var bloodNrml = normal;
        var rot = 360f - Mathf.Atan2 (bloodNrml.z, bloodNrml.x) * Mathf.Rad2Deg;
        var bloodIns = Object.Instantiate<Blood> (bloodPrefab, bleedPoint, Quaternion.Euler (0f, rot, 0f));
        Object.Destroy (bloodIns.gameObject, bloodIns.particleSystem.main.startLifetimeMultiplier);
        return bloodIns;
    }

    public static Blood BleedOut (Blood bloodPrefab, Quaternion rotation, Vector3 bleedPoint)
    {
        var headRot = rotation;
        var headEuler = headRot.eulerAngles;
        var bloodInsRot = Quaternion.Euler (headEuler.x, headEuler.y + 90f, headEuler.z);
        var bloodIns = Object.Instantiate<Blood> (bloodPrefab, bleedPoint, bloodInsRot);
        Object.Destroy (bloodIns.gameObject, bloodIns.particleSystem.main.startLifetimeMultiplier);
        return bloodIns;
    }

    public static Vector3 GetDirection(Transform transform, Vector3 axis)
    {
        return transform.rotation * axis;
    }
}
