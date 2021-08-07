using Net;
using UnityEngine;

public class NetHand : MonoBehaviour
{
  [Range(0, 2)]
  public float maximumRange = 1f;
  [SerializeField]
  Transform _arm;
  DotSightController _dotSightController;
  DotSight _dotSight;
  float _maximumDistance = 4f;

  [SerializeField]
  NetIdentity _netIdentity;

  Vector3 _lastLocalPosition;

  void Start()
  {
    transform.position = _arm.position;
    if (_netIdentity.isLocal)
    {
      _dotSightController = FindObjectOfType<DotSightController>();
      if (_dotSightController)
      {
        _dotSight = _dotSightController.dotSight;
      }
    }
    if (_netIdentity.isClient)
    {
      _netIdentity.onMessageReceived += (eventName, message) =>
      {
        if (eventName == "hand_move_in_range")
        {
          var receivedMessage = Utility.Deserialize<MoveInRangeJson>(message);
          var localPosition = receivedMessage.localPosition;
          transform.localPosition = Utility.PositionArrayToVector3(transform.localPosition, localPosition);
        }
      };
    }
  }

  void Update()
  {
    if (_netIdentity.isLocal)
    {
      MoveInRange();
      EmitMoveInRange();
    }
  }

  void MoveInRange()
  {
    var distance = Vector3.Distance(_dotSight.GetCurrentPoint(), _arm.position);
    var rangeForMoving = distance / _maximumDistance;
    var pos = transform.localPosition;
    pos.x = Mathf.Clamp(rangeForMoving, 0, maximumRange);
    pos.z = 0;
    transform.localPosition = pos;
    _lastLocalPosition = transform.localPosition;
  }

  void EmitMoveInRange()
  {
    if (_lastLocalPosition != transform.localPosition)
    {
      _netIdentity.EmitMessage("hand_move_in_range", new MoveInRangeJson
      {
        localPosition = Utility.Vector3ToPositionArray(transform.localPosition)
      });
    }
  }
}

public struct MoveInRangeJson
{
  public float[] localPosition;
}
