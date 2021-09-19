using UnityEngine;

public class HeatUI : MonoBehaviour
{
  Heat _heat;

  [SerializeField]
  Canvas _canvas;

  [SerializeField]
  RectTransform _rectTransform;

  void Start()
  {

  }

  void Update()
  {
    var normalizedLife = 1f - _heat.heat;
    transform.localScale = new Vector3(normalizedLife, transform.localScale.y, transform.localScale.z);
  }

  public void SetHeat(Heat heat)
  {
    _heat = heat;
  }

  public void Visible(bool visible)
  {
    _rectTransform.gameObject.SetActive(visible);
  }
}