using UnityEngine;

public class SelectedServer : MonoBehaviour
{
  public ServerObject server;

  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
}
