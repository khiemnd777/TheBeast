using Net;
using UnityEngine;

public class NetHand : MonoBehaviour
{
  public HolderSide holderSide = HolderSide.Right;
  [Range(0, 2)]
  public float maximumRange = 1f;
  [SerializeField]
  Transform _arm;
  DotSightController _dotSightController;
  DotSight _dotSight;
  float _maximumDistance = 4f;

  [SerializeField]
  float _emitMessageInterval = .2f;

  Cooldown _emitMessageCooldown;

  [SerializeField]
  NetIdentity _netIdentity;

  Vector3 _lastLocalPosition;

  void Start()
  {
    if (_netIdentity.isLocal)
    {
      _dotSightController = FindObjectOfType<DotSightController>();
      if (_dotSightController)
      {
        _dotSight = _dotSightController.dotSight;
      }
    }
    if (_netIdentity.isClient && !_netIdentity.isLocal)
    {
      _netIdentity.onMessageReceived += OnReceivedMoveInRange;
    }
    if (_netIdentity.isLocal)
    {
      _emitMessageCooldown = new Cooldown(EmitMoveInRange);
    }
  }

  void Update()
  {
    if (_netIdentity.isLocal)
    {
      MoveInRange();
      _emitMessageCooldown.Count(_emitMessageInterval);
      _emitMessageCooldown.Execute();
    }
  }

  void MoveInRange()
  {
    var distance = Vector3.Distance(_dotSight.GetCurrentPoint(), _arm.position);
    var rangeForMoving = distance / _maximumDistance;
    var pos = transform.localPosition;
    pos.x = Mathf.Clamp(rangeForMoving, 0, maximumRange);
    // pos.z = 0;
    transform.localPosition = pos;
  }

  void EmitMoveInRange()
  {
    if (_lastLocalPosition != transform.localPosition && transform.localPosition != Vector3.zero)
    {
      _netIdentity.EmitMessage("hand_move_in_range", new MoveInRangeJson
      {
        side = (int)holderSide,
        localPosition = Utility.Vector3ToPositionArray(transform.localPosition)
      });
      _lastLocalPosition = transform.localPosition;
    }
  }

  void OnReceivedMoveInRange(string eventName, string message)
  {
    if (eventName == "hand_move_in_range")
    {
      var receivedMessage = Utility.Deserialize<MoveInRangeJson>(message);
      if (receivedMessage.side == (int)holderSide)
      {
        var localPosition = receivedMessage.localPosition;
        var expectedPos = Utility.PositionArrayToVector3(transform.localPosition, localPosition);
        var pos = transform.localPosition;
        pos.x = expectedPos.x;
        transform.localPosition = pos;
      }
    }
  }
}

public struct MoveInRangeJson
{
  public int side;
  public float[] localPosition;
}
