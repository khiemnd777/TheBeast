using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Camera theCamera;
  public Transform target;
  public float smoothSpeed = .125f;
  public Vector3 offset;
  public Boundary bound;
  public BoundingBox limitedBoundingBox;

  public Vector3 deltaPosition;
  Vector3 _lastPosition;
  int _previousTargetId;

  void Update()
  {
    if (target)
    {
      if (limitedBoundingBox.size == Vector3.zero)
      {
        SetLimitedBoundingBox(bound.boundary);
      }
      if (limitedBoundingBox.centerTarget)
      {
        var desiredPos = target.position + offset;
        var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = Utility.CameraInBound(theCamera, limitedBoundingBox.centerTarget.position, limitedBoundingBox.size.x, limitedBoundingBox.size.z, smoothedPos);
        if (_previousTargetId == this.target.GetInstanceID())
        {
          deltaPosition = _lastPosition - transform.position;
          _lastPosition = transform.position;
        }
        else
        {
          _lastPosition = Vector3.zero;
          _previousTargetId = this.target.GetInstanceID();
        }
      }
    }
  }

  public void SetLimitedBoundingBox(BoundingBox limitedBoundingBox)
  {
    this.limitedBoundingBox = limitedBoundingBox;
  }

  public void SetDefaultLimitedBoundingBox()
  {
    this.limitedBoundingBox = bound.boundary;
  }

  /// <summary>
  /// Set a specific target for the camera tracking after.
  /// </summary>
  /// <param name="target"></param>
  public void SetTarget(Transform target)
  {
    this.target = target;
    _previousTargetId = this.target.GetInstanceID();
  }
}
