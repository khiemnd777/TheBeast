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
    Text playBtnText;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      playBtnText = playButton.GetComponentInChildren<Text>();
      netRegistrar = FindObjectOfType<NetRegistrar>();
      _networkManager = NetworkManagerCache.networkManager;
      _networkManager.onClientRegisterFinished += (NetObjectJSON netObjJson) =>
        {
          // Detects the local player at the client-side.
          if (netObjJson.clientId.Equals(_networkManager.clientId))
          {
            this.gameObject.SetActive(false);
          }
        };
    }

    /// <summary>
    /// Fired by Play button onclick event.
    /// </summary>
    public void Play()
    {
      playButton.interactable = false;
      nicknameInputField.interactable = false;
      // To begin with, a connection is established.
      print("Connecting...");
      playBtnText.text = "Connecting...";
      netRegistrar.Register("player", nicknameInputField.text);
    }
  }
}
