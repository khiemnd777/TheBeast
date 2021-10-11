using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
  public class GuidePanelUI : MonoBehaviour
  {
    [SerializeField]
    RectTransform _playUIRectTransform;

    [SerializeField]
    RectTransform _guideRectTransform;


    Text playBtnText;


    public void Back()
    {
      _playUIRectTransform.gameObject.SetActive(true);
      _guideRectTransform.gameObject.SetActive(false);
    }
  }
}
