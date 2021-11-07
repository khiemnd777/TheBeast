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
      Application.OpenURL($"{baseUrl}{UnityWebRequest.EscapeURL(url)}");
    }
  }
}
