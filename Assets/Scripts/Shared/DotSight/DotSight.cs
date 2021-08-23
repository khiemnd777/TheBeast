using UnityEngine;

public class DotSight : MonoBehaviour
{
  [SerializeField]
  Camera _theCamera;
  [SerializeField]
  [Range (0f, 1f)]
  public float sensitivity = 1f;
  [SerializeField]
  Vector3 _offset;
  Transform _cachedTransform;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake ()
  {
    _cachedTransform = transform;
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update ()
  {
    if (_theCamera)
    {
      _cachedTransform.position = Utility.TranslateByMouseInsideScreen (GetMouseX (), GetMouseY (), _cachedTransform.position, _theCamera);
    }
  }

  float GetMouseX ()
  {
    return Input.GetAxis ("Mouse X") * sensitivity;
  }

  float GetMouseY ()
  {
    return Input.GetAxis ("Mouse Y") * sensitivity;
  }

  /// <summary>
  /// Initialize some stuck before appears the dot sight.
  /// </summary>
  /// <param name="mainCamera"></param>
  public void Init (Camera mainCamera)
  {
    SetMainCamera (mainCamera);
    DisableCursor ();
    ReplaceFromMousePosition ();
  }

  /// <summary>
  /// Disables the cursor before appears the dot sight.
  /// </summary>
  void DisableCursor ()
  {
    Cursor.visible = false;
  }

  /// <summary>
  /// Replace the dot sight position from the mouse position.
  /// </summary>
  void ReplaceFromMousePosition ()
  {
    var mousePoint = _theCamera.ScreenToWorldPoint (Input.mousePosition);
    _cachedTransform.position = mousePoint;
  }

  /// <summary>
  /// Set the main camera for calculation of the dot sight translation at the first time.
  /// </summary>
  /// <param name="mainCamera"></param>
  void SetMainCamera (Camera mainCamera)
  {
    _theCamera = mainCamera;
  }

  /// <summary>
  /// Get the current position of dot sight.
  /// </summary>
  /// <returns></returns>
  public Vector3 GetCurrentPoint ()
  {
    return _cachedTransform.position;
  }

  /// <summary>
  /// Normalize the direction vector to a specific position.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  public Vector3 NormalizeFromPoint (Vector3 point)
  {
    var direction = GetDirectionFromPoint (point);
    direction.Normalize ();
    return direction;
  }

  /// <summary>
  /// Normalize the direction vector to a specific point.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  public Point NormalizeFromPoint (Point point)
  {
    var direction = GetDirectionFromPoint (point);
    direction.Normalize ();
    return direction;
  }

  /// <summary>
  /// Get the direction vector from a specific position.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  public Vector3 GetDirectionFromPoint (Vector3 point)
  {
    var currentPoint = GetCurrentPoint ();
    return currentPoint - point;
  }

  /// <summary>
  /// Get the direction vector from a specific point.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  public Point GetDirectionFromPoint (Point point)
  {
    var currentPoint = GetCurrentPoint ();
    return Point.FromVector3 (currentPoint) - point;
  }
}
