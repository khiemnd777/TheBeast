using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
  public float size;
  public float range;
  public LayerMask layerMask;
  bool _isSetup;

  void FixedUpdate()
  {
    if (_isSetup)
    {
      var colliders = Physics.OverlapSphere(this.transform.position, size, layerMask);
    }
  }

  public void Setup(MeleeColliderOptions options)
  {
    this.size = options.size;
    this.range = options.range;
    this.layerMask = options.layerMask;
    this.transform.localPosition = new Vector3(range, 0f, 0f);
    _isSetup = true;
  }

  public void Reset()
  {
    _isSetup = false;
    this.size = 0f;
    this.range = 0f;
    this.layerMask = -1;
    this.transform.localPosition = Vector3.zero;
  }

  void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(this.transform.position, size);
  }
}

public struct MeleeColliderOptions
{
  public float size;
  public float range;
  public LayerMask layerMask;
}
