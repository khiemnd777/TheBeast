using UnityEngine;
using UnityEngine.UI;

public class GunsSelectUI : MonoBehaviour
{
  public Button nextBtn;
  public Button prevBtn;
  public GunUI[] guns;

  int _index = 0;

  public int index { get => _index; }

  public GunUI gunUI { get => guns[_index]; }

  void Start()
  {
    nextBtn.onClick.AddListener(() =>
    {
      _index++;
      if (_index == guns.Length)
      {
        _index = 0;
      }
      HideAllGuns();
      VisibleGun(_index, true);
    });

    prevBtn.onClick.AddListener(() =>
    {
      _index--;
      if (_index == -1)
      {
        _index = guns.Length - 1;
      }
      HideAllGuns();
      VisibleGun(_index, true);
    });
  }

  void HideAllGuns()
  {
    foreach (var gun in guns)
    {
      gun.gameObject.SetActive(false);
    }
  }

  void VisibleGun(int index, bool visible)
  {
    var gun = guns[index];
    if (gun)
    {
      gun.gameObject.SetActive(visible);
    }
  }
}
