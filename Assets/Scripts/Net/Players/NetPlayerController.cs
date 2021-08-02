using UnityEngine;

namespace Net
{
  [RequireComponent (typeof (NetIdentity), typeof (NetTransform))]
  public class NetPlayerController : MonoBehaviour
  {
    public float initSpeed = 2f;
    [Range (0f, 1f)]
    public float sprintSpeed = 1f;
    [Range (0f, 1f)]
    public float walkSpeed = .5f;
    SocketNetworkManager _socketNetworkManager;
    NetIdentity _netIdentity;
    NetTransform _netTransform;
    MovingCalculator _movingCalculator;
    SpeedCalculator _speedCalculator;
    Transform _cachedTransform;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake ()
    {
      _netIdentity = GetComponent<NetIdentity> ();
      _netTransform = GetComponent<NetTransform> ();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start ()
    {
      _cachedTransform = transform;
      
      if (_netIdentity.isLocal)
      {
        _movingCalculator = new MovingCalculator (Point.FromVector3 (_cachedTransform.position));
        _speedCalculator = new SpeedCalculator ();
      }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate ()
    {
      if (_netIdentity.isLocal)
      {
        var horizontalDirection = GetHorizontalDirection ();
        var verticalDirection = GetVerticalDirection ();
        _movingCalculator
          .Calculate (horizontalDirection, verticalDirection);
        _speedCalculator
          .SetSpeeds (initSpeed, sprintSpeed, walkSpeed)
          .Calculate (SpeedType.Sprint);
        _netTransform
          .Velocity (_movingCalculator.direction, _speedCalculator.speed);
      }
    }

    /// <summary>
    /// Get horizontal direction (x-axis).
    /// </summary>
    /// <returns></returns>
    float GetHorizontalDirection ()
    {
      return Input.GetAxisRaw ("Horizontal");
    }

    /// <summary>
    /// Get vertical direction (y-axis).
    /// </summary>
    /// <returns></returns>
    float GetVerticalDirection ()
    {
      return Input.GetAxisRaw ("Vertical");
    }

    /// <summary>
    /// Rotation
    /// </summary>
    /// <param name="rotation"></param>
    public void Rotate (Quaternion rotation)
    {
      _netTransform.Rotate (rotation);
    }
  }
}
