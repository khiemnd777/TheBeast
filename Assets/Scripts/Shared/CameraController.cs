using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Camera theCamera;
  public Transform target;
  public float smoothSpeed = .125f;
  public Vector3 offset;
  public Boundary bound;
  public BoundingBox limitedBoundingBox;

  void Update()
  {
    if (target)
    {
      if (limitedBoundingBox.size == Vector3.zero)
      {
        SetLimitedBoundingBox(bound.boundary);
      }
      var desiredPos = target.position + offset;
      var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
      transform.position = Utility.CameraInBound(theCamera, limitedBoundingBox.center, limitedBoundingBox.size.x, limitedBoundingBox.size.z, smoothedPos);
    }
  }

  public void SetLimitedBoundingBox(BoundingBox limitedBoundingBox)
  {
    this.limitedBoundingBox = limitedBoundingBox;
  }

  /// <summary>
  /// Set a specific target for the camera tracking after.
  /// </summary>
  /// <param name="target"></param>
  public void SetTarget(Transform target)
  {
    this.target = target;
  }
}
