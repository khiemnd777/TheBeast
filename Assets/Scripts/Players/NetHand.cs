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
  }

  void Update()
  {
    if (_netIdentity.isLocal)
    {
      MoveInRange();
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
  }
}
