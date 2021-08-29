using UnityEngine;

public class TestFov : MonoBehaviour, IFieldOfViewVisualizer
{
  public Transform target;

  public void OnTargetEnterFov()
  {
    if (target)
    {
      target.GetComponent<Renderer>().enabled = true;
    }
    Debug.Log("Object entered!");
  }

  public void OnTargetLeaveFov()
  {
    if (target)
    {
      target.GetComponent<Renderer>().enabled = false;
    }
    Debug.Log("Object left!");
  }
}