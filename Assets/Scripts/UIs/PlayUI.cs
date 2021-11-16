using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
  public class PlayUI : MonoBehaviour
  {
    [SerializeField]
    InputField nicknameInputField;

    [SerializeField]
    Button playButton;

    NetRegistrar netRegistrar;

    NetworkManager _networkManager;

    LocalPlayerManager _localPlayerManager;

    RectTransform _rectTransform;

    [SerializeField]
    RectTransform _playUIRectTransform;

    [Header("Guide")]
    [SerializeField]
    RectTransform _guideRectTransform;

    [SerializeField]
    Button _guideButton;

    [SerializeField]
    Button _fullscreenButton;

    [SerializeField]
    Button _exitFullscreenButton;

    [Header("Guns Select")]
    [SerializeField]
    GunsSelectUI _gunsSelectUI;

    Text playBtnText;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      _rectTransform = GetComponent<RectTransform>();
      playBtnText = playButton.GetComponentInChildren<Text>();
      netRegistrar = FindObjectOfType<NetRegistrar>();
      _localPlayerManager = FindObjectOfType<LocalPlayerManager>();
      _localPlayerManager.onPlayerSetup += (Player player) =>
      {
        if (player.isLocal)
        {
          player.onDead += OnPlayerDead;
        }
      };
      _networkManager = NetworkManagerCache.networkManager;
      _networkManager.onClientRegisterFinished += (NetObjectJSON netObjJson) =>
        {
          // Detects the local player at the client-side.
          if (netObjJson.clientId.Equals(_networkManager.clientId))
          {
            StartCoroutine(Hide());
          }
        };
      StartCoroutine(Show());
    }

    void OnPlayerDead()
    {
      playButton.interactable = true;
      nicknameInputField.interactable = true;
      playBtnText.text = "Play";
      gameObject.SetActive(true);
      StartCoroutine(Show());
#if UNITY_WEBGL
      SettingsFactory.PlayerDead();
#endif
    }

    /// <summary>
    /// Fired by Play button on-click event.
    /// </summary>
    public void Play()
    {
      playButton.interactable = false;
      _guideButton.interactable = false;
      nicknameInputField.interactable = false;
      // To begin with, a connection is established.
      print("Connecting...");
      playBtnText.text = "Connecting...";
      netRegistrar.Register(_gunsSelectUI.gunUI.prefabName, nicknameInputField.text);
#if UNITY_WEBGL
      SettingsFactory.Play();
#endif
    }

    public void Fullscreen()
    {
      _fullscreenButton.gameObject.SetActive(false);
      _exitFullscreenButton.gameObject.SetActive(true);
#if UNITY_WEBGL
      SettingsFactory.Fullscreen();
#endif
    }

    public void Windowscreen()
    {
      _fullscreenButton.gameObject.SetActive(true);
      _exitFullscreenButton.gameObject.SetActive(false);
#if UNITY_WEBGL
      SettingsFactory.Windowscreen();
#endif
    }

    public void ShowGuidePanel()
    {
      _guideRectTransform.gameObject.SetActive(true);
      _playUIRectTransform.gameObject.SetActive(false);
    }

    IEnumerator Hide()
    {
      _rectTransform.localScale = Vector3.one;
      var t = 0f;
      while (t <= 1f)
      {
        t += Time.deltaTime / .5f;
        var scale = Mathf.Lerp(1f, 0f, t);
        _rectTransform.localScale = Vector3.one * scale;
        yield return null;
      }
      gameObject.SetActive(false);
    }

    IEnumerator Show()
    {
      _rectTransform.localScale = Vector3.zero;
      var t = 0f;
      while (t <= 1f)
      {
        t += Time.deltaTime / .5f;
        var scale = Mathf.Lerp(0f, 1f, t);
        _rectTransform.localScale = Vector3.one * scale;
        yield return null;
      }
    }
  }
}
