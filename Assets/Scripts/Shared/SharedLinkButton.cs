using UnityEngine;
using UnityEngine.Networking;

public class SharedLinkButton : MonoBehaviour
{
  public string baseUrl;
  public string url;

  public void Click()
  {
    if (!string.IsNullOrEmpty(url))
    {
      var link = $"{baseUrl}{UnityWebRequest.EscapeURL(url)}";
#if UNITY_WEBGL
      OpenUrlWindowFactory.OpenUrlWindow(link);
#else
      Application.OpenURL(link);
#endif
    }
  }
}
