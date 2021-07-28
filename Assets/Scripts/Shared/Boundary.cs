using UnityEngine;

public class Boundary : MonoBehaviour
{
  public float width = 180;
  public float height = 120;

  BoxCollider boxCollider;

  static Boundary pInstance;
  public static Boundary instance
  {
    get
    {
      return pInstance ?? (pInstance = FindObjectOfType<Boundary> ());
    }
  }

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start ()
  {
    boxCollider = GetComponent<BoxCollider> ();
    boxCollider.size = new Vector3 (width, 0f, height);
  }
}
