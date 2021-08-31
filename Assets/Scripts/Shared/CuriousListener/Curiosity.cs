using UnityEngine;

public class Curiosity : MonoBehaviour
{
  [System.NonSerialized]
  public string curiousIdentity;
  public float radius;
  public float lifetime = 4f;
  public SphereCollider footstepCollider;

  void Start()
  {
    if (footstepCollider)
    {
      footstepCollider.radius = radius;
    }
    Destroy(gameObject, lifetime);
  }
}
