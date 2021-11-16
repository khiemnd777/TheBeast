using UnityEngine;

public class LinkButton : MonoBehaviour
{
  public string url;

  public void Click()
  {
    if (!string.IsNullOrEmpty(url))
    {
#if UNITY_WEBGL
      OpenUrlWindowFactory.OpenUrlWindow(url);
#else
      Application.OpenURL(url);
#endif
    }
  }
}
