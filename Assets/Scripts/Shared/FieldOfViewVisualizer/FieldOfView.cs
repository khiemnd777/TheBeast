using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
  public float viewRadius;
  [Range(0, 360)]
  public float viewAngle;

  public FieldOfViewDirection direction;

  [Space]
  public Transform affectedTransform;

  [Space]
  public float referredAngle;

  [Space]
  public LayerMask targetMask;
  public LayerMask obstacleMask;

  [HideInInspector]
  public List<Transform> visibleTargets = new List<Transform>();

  public float meshResolution;
  public int edgeResolveIterations;
  public float edgeDstThreshold;

  public float maskCutawayDst = .1f;

  public MeshFilter viewMeshFilter;

  public SphereCollider detectedLeaveCollider;
  Mesh viewMesh;

  void Start()
  {
    if (!affectedTransform)
    {
      affectedTransform = transform;
    }
    viewMesh = new Mesh();
    viewMesh.name = "View Mesh";
    viewMeshFilter.mesh = viewMesh;

    StartCoroutine("FindTargetsWithDelay", .2f);
  }

  IEnumerator FindTargetsWithDelay(float delay)
  {
    while (true)
    {
      yield return new WaitForSeconds(delay);
      FindVisibleTargets();
    }
  }

  void LateUpdate()
  {
    DrawFieldOfView();
    // Set radius for detected leave collder
    if (detectedLeaveCollider)
    {
      detectedLeaveCollider.radius = viewRadius;
    }
  }

  void OnTriggerExit(Collider other)
  {
    var fov = other.GetComponent<IFieldOfViewVisualizer>() ?? other.GetComponentInParent<IFieldOfViewVisualizer>();
    if (fov != null)
    {
      fov.OnTargetLeaveFov();
    }
  }

  void FindVisibleTargets()
  {
    visibleTargets.Clear();
    var targetsInViewRadius = Physics.OverlapSphere(affectedTransform.position, viewRadius, targetMask);
    for (var i = 0; i < targetsInViewRadius.Length; i++)
    {
      var target = targetsInViewRadius[i].transform;
      var dirToTarget = (target.position - affectedTransform.position).normalized;
      var targetAngle = viewAngle / 2;
      if (Vector3.Angle(GetAffectedTransformDirection(direction), dirToTarget) < targetAngle)
      {
        var dstToTarget = Vector3.Distance(affectedTransform.position, target.position);
        if (!Physics.Raycast(affectedTransform.position, dirToTarget, dstToTarget, obstacleMask))
        {
          var fov = target.GetComponent<IFieldOfViewVisualizer>() ?? target.GetComponentInParent<IFieldOfViewVisualizer>();
          if (fov != null)
          {
            fov.OnTargetEnterFov();
          }
          visibleTargets.Add(target);
        }
      }
      else
      {
        var fov = target.GetComponent<IFieldOfViewVisualizer>() ?? target.GetComponentInParent<IFieldOfViewVisualizer>();
        if (fov != null)
        {
          fov.OnTargetLeaveFov();
        }
      }
    }
  }

  Vector3 GetAffectedTransformDirection(FieldOfViewDirection direction)
  {
    switch (direction)
    {
      case FieldOfViewDirection.right:
        {
          return affectedTransform.right;
        }
      case FieldOfViewDirection.up:
        {
          return affectedTransform.up;
        }
      case FieldOfViewDirection.forward:
      default:
        {
          return affectedTransform.forward;
        }
    }
  }

  void DrawFieldOfView()
  {
    var stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
    var stepAngleSize = viewAngle / stepCount;
    var viewPoints = new List<Vector3>();
    var oldViewCast = new ViewCastInfo();
    for (var i = 0; i <= stepCount; i++)
    {
      var angle = affectedTransform.eulerAngles.y - (viewAngle / 2) + stepAngleSize * i;
      var newViewCast = ViewCastInfo.GetViewCast(affectedTransform, angle + referredAngle, viewRadius, obstacleMask, true);

      if (i > 0)
      {
        var edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold;
        if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
        {
          var edge = EdgeInfo.FindEdge(affectedTransform, oldViewCast, newViewCast, viewRadius, edgeResolveIterations, edgeDstThreshold, referredAngle, obstacleMask, true);
          if (edge.pointA != Vector3.zero)
          {
            viewPoints.Add(edge.pointA);
          }
          if (edge.pointB != Vector3.zero)
          {
            viewPoints.Add(edge.pointB);
          }
        }
      }
      viewPoints.Add(newViewCast.point);
      oldViewCast = newViewCast;
    }

    var vertexCount = viewPoints.Count + 1;
    var vertices = new Vector3[vertexCount];
    var triangles = new int[(vertexCount - 2) * 3];

    vertices[0] = Vector3.zero;
    for (var i = 0; i < vertexCount - 1; i++)
    {
      vertices[i + 1] = affectedTransform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDst;

      if (i < vertexCount - 2)
      {
        triangles[i * 3] = 0;
        triangles[i * 3 + 1] = i + 1;
        triangles[i * 3 + 2] = i + 2;
      }
    }

    viewMesh.Clear();

    viewMesh.vertices = vertices;
    viewMesh.triangles = triangles;
    viewMesh.RecalculateNormals();
  }
}
