using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectileFlash : MonoBehaviour
{
  [SerializeField]
  SpriteRenderer _renderer;

  [SerializeField]
  Sprite[] _flashes;
  float _deltaTimeDisappeared;
  Color _originalColor;

  // Start is called before the first frame update
  void Start()
  {
    if (_renderer)
    {
      if (_flashes != null && _flashes.Length > 0)
      {
        var index = Random.Range(0, _flashes.Length - 1);
        _renderer.sprite = _flashes[index];
      }
      _originalColor = _renderer.color;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (_renderer)
    {
      _deltaTimeDisappeared += Time.deltaTime / .125f;
      if (_deltaTimeDisappeared <= 1f)
      {
        // Transparent zero
        var aLerp = Mathf.Lerp(1f, 0f, _deltaTimeDisappeared);
        _renderer.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, aLerp);

        // Scale zero
        var scaleLerp = Mathf.Lerp(1f, 0f, _deltaTimeDisappeared);
        transform.localScale = Vector3.one * scaleLerp;
        return;
      }
    }
    Destroy(gameObject);
  }
}
