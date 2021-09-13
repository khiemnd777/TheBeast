using Net;
using UnityEngine;
using UnityEngine.UI;

public class DotSightController : MonoBehaviour
{
  Settings _settings;

  [SerializeField]
  DotSight _dotSightPrefab;

  [SerializeField]
  Camera _theCamera;

  [SerializeField]
  CameraController _cameraController;

  [SerializeField]
  Slider _mouseSensitivitySlider;

  [SerializeField]
  NetIdentity _netIdentity;
  NetPlayerController _netPlayerController;

  DotSight _dotSight;

  void Start()
  {
    _settings = Settings.instance;
  }

  void Update()
  {
    if (_netIdentity && _netIdentity.isLocal)
    {
      if (_netPlayerController && _dotSight)
      {
        var dotSightNormalFromPoint = _dotSight.NormalizeFromPoint(_netPlayerController.transform.position);
        var destinationRotation = Utility.RotateToDirection(dotSightNormalFromPoint, Vector3.up);
        _netPlayerController.Rotate(destinationRotation);
      }
    }
  }

  public void VisibleCursor(bool visible)
  {
    Cursor.visible = visible;
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
    var insDotSight = Instantiate<DotSight>(_dotSightPrefab, Vector3.zero, Quaternion.identity);
    insDotSight.Init(_cameraController);
    insDotSight.sensitivity = _mouseSensitivitySlider.value;
    _dotSight = insDotSight;
  }

  public void SetGlobally()
  {
    _dotSight.local = false;
  }

  public void SetLocally()
  {
    _dotSight.local = true;
  }

  public void ResetSensitivity()
  {
    SetSensitivity(_mouseSensitivitySlider.value);
  }

  public void SetSensitivity(float sensitivity)
  {
    _dotSight.sensitivity = sensitivity;
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
