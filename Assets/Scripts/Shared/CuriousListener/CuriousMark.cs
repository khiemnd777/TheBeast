using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriousMark : MonoBehaviour
{
  [System.NonSerialized]
  public Transform target;

  [SerializeField]
  Transform _mark;
  [SerializeField]
  float _timeMarkPerforming = .5f;

  [SerializeField]
  float _lifetime = 4f;

  CameraController _cameraController;

  Vector3 _originalPoint;

  void Start()
  {
    _cameraController = FindObjectOfType<CameraController>();
    _originalPoint = this.transform.position;
    StartCoroutine("PerformMark", _timeMarkPerforming);
    Destroy(gameObject, _lifetime);
  }

  void Update()
  {
    if (target)
    {
      var intersectedPoint = Utility.LineLineIntersection(_cameraController.theCamera, _cameraController.theCamera.transform.position, new Point(_cameraController.theCamera.transform.position.x, _cameraController.theCamera.transform.position.z), new Point(_originalPoint.x, _originalPoint.z), 1f, .5f, .5f, .5f);
      if (intersectedPoint.x != float.MaxValue && intersectedPoint.y != float.MaxValue)
      {
        this.transform.position = new Vector3(intersectedPoint.x, 3f, intersectedPoint.y);
      }
      else
      {
        this.transform.position = _originalPoint;
      }
    }
  }

  void OnDrawGizmos()
  {
    if (target)
    {
      var center = target.position;
      var halfHeight = _cameraController.theCamera.orthographicSize;
      var halfWidth = halfHeight * Screen.width / Screen.height;
      var a1 = new List<Vector3>{
        new Vector3(center.x - halfWidth, 0f, center.z - halfHeight),
        // new Vector3(center.x + halfWidth, 0f, center.z - halfHeight),
        new Vector3(center.x - halfWidth, 0f, center.z + halfHeight),
        // new Vector3(center.x - halfWidth, 0f, center.z + halfHeight)
      };
      var a2 = new List<Vector3>{
        new Vector3(center.x + halfWidth, 0f, center.z - halfHeight),
        // new Vector3(center.x + halfWidth, 0f, center.z + halfHeight),
        new Vector3(center.x + halfWidth, 0f, center.z + halfHeight),
        // new Vector3(center.x - halfWidth, 0f, center.z - halfHeight),
      };
      Gizmos.color = Color.blue;
      for (var i = 0; i < a1.Count; i++)
      {
        Gizmos.DrawLine(a1[i], a2[i]);
      }
      Gizmos.DrawLine(center, _originalPoint);
    }
  }

  IEnumerator PerformMark(float timeMarkPerforming)
  {
    var originScale = Vector3.zero;
    var destScale = Vector3.one;
    var t = 0f;
    while (t <= 1f)
    {
      t += Time.fixedDeltaTime / timeMarkPerforming;
      _mark.localScale = Vector3.Lerp(originScale, destScale, t);
      yield return new WaitForFixedUpdate();
    }
  }
}
