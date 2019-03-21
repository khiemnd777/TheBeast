using UnityEngine;

public class Utilities
{
    public static Vector3 CameraInBound (Camera camera, BoxCollider2D bound, Vector3 position)
    {
        var min = bound.bounds.min;
        var max = bound.bounds.max;
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var x = Mathf.Clamp (position.x, min.x + halfWidth, max.x - halfWidth);
        var y = Mathf.Clamp (position.y, min.y + halfHeight, max.y - halfHeight);
        return new Vector3 (x, y, position.z);
    }
}
