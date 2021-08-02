using Net;
using Net.Socket;
using UnityEngine;

public class NetAutomaticTransform : MonoBehaviour
{
  Settings settings;
  ISocketWrapper socket;
  Transform cachedTransform;
  Vector3 lastPosition;
  NetIdentity netIdentity;

  public bool canSendTranslateMessage
  {
    get
    {
      return cachedTransform.position != lastPosition;
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake ()
  {
    settings = Settings.instance;
    socket = SocketNetworkManagerCache.socket;
    netIdentity = GetComponent<NetIdentity> ();
  }

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start ()
  {
    cachedTransform = transform;
  }

  /// <summary>
  /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update ()
  {
    if (!settings.isServer) return;
    if (!canSendTranslateMessage) return;
    var point = Point.FromVector3 (cachedTransform.position);
    var netPosition = new NetPositionJSON (netIdentity.id, point);
    socket.Emit (Constants.EVENT_SERVER_PLAYER_TRANSLATE, netPosition);
    lastPosition = cachedTransform.position;
  }

  /// <summary>
  /// Moves the transform in the direction and distance of translation.
  /// </summary>
  /// <param name="point"></param>
  public void Translate (Point point)
  {
    if (!settings.isClient) return;
    cachedTransform.position = Point.ToVector3 (cachedTransform.position, point);
  }
}
