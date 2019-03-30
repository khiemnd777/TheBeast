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
}
