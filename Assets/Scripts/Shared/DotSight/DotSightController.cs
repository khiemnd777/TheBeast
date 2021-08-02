using Net;
using UnityEngine;
using UnityEngine.UI;

public class DotSightController : MonoBehaviour
{
  [SerializeField]
  DotSight _dotSightPrefab;

  [SerializeField]
  Camera _theCamera;

  [SerializeField]
  Slider _mouseSensitivitySlider;

  [SerializeField]
  NetIdentity _netIdentity;
  NetPlayerController _netPlayerController;

  DotSight _dotSight;

  void Awake()
  {
    Cursor.visible = false;
  }

  void Update()
  {
    if (_netPlayerController && _dotSight)
    {
      var dotSightNormalFromPoint = _dotSight.NormalizeFromPoint(_netPlayerController.transform.position);
      var destinationRotation = Utility.RotateToDirection(dotSightNormalFromPoint, Vector3.up);
      _netPlayerController.Rotate(destinationRotation);
    }
  }

  public Vector3 GetPosition()
  {
    return transform.position;
  }

  public Vector3 NormalizeFromPoint(Vector3 point)
  {
    var pos = GetPosition();
    var dir = pos - point;
    dir.Normalize();
    return dir;
  }

  /// <summary>
  /// Get the dot sight that runs on the local machine.
  /// </summary>
  /// <value></value>
  public DotSight dotSight
  {
    get { return _dotSight; }
  }

  /// <summary>
  /// Initialize the dot sight from the prefab.
  /// </summary>
  public void InitDotSight()
  {
    var insDotSight = Instantiate<DotSight>(_dotSightPrefab, Vector3.zero, Quaternion.identity, _theCamera.transform);
    insDotSight.Init(_theCamera);
    insDotSight.sensitivity = _mouseSensitivitySlider.value;
    _dotSight = insDotSight;
  }

  /// <summary>
  /// Set player through NetIdentity object for mapping with the dot sight.
  /// </summary>
  /// <param name="netIdentity"></param>
  public void SetPlayer(NetIdentity netIdentity)
  {
    _netIdentity = netIdentity;
    _netPlayerController = _netIdentity.GetComponent<NetPlayerController>();
  }
}
