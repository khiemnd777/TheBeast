using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
  [Range(0, 2)]
  public float maximumRange = 1f;
  [SerializeField]
  Transform _arm;
  DotSightController _dotSightController;
  DotSight _dotSight;
  float _maximumDistance = 4f;

  void Start()
  {
    transform.position = _arm.position;
    _dotSightController = FindObjectOfType<DotSightController>();
    if (_dotSightController)
    {
      _dotSight = _dotSightController.dotSight;
    }
  }

  void Update()
  {
    // MoveInRange ();
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
