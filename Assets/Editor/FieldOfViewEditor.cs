using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
  void OnSceneGUI()
  {
    var fov = (FieldOfView)target;
    if (!fov.enabled) return;
    var fovTransform = fov.affectedTransform;
    var referredAngle = fov.referredAngle;

    Handles.color = Color.white;
    Handles.DrawWireArc(fovTransform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

    var viewAngleA = FieldOfViewUtility.DirectionFromAngle(fovTransform, -fov.viewAngle / 2 + referredAngle, false);
    var viewAngleB = FieldOfViewUtility.DirectionFromAngle(fovTransform, fov.viewAngle / 2 + referredAngle, false);

    Handles.DrawLine(fovTransform.position, fovTransform.position + viewAngleA * fov.viewRadius);
    Handles.DrawLine(fovTransform.position, fovTransform.position + viewAngleB * fov.viewRadius);

    Handles.color = Color.red;
    foreach (var visibleTarget in fov.visibleTargets)
    {
      Handles.DrawLine(fovTransform.position, visibleTarget.position);
    }
  }
}
