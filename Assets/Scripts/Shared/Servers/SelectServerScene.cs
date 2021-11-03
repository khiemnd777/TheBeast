using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectServerScene : MonoBehaviour
{
  [SerializeField]
  string _nextScene;

  [SerializeField]
  ServerSO _server;

  [SerializeField]
  SelectedServer _selectedServer;

  void Start()
  {
    if (Debug.isDebugBuild)
    {
      _selectedServer.server = _server.servers[0];
    }
    else
    {
#if DEVELOPMENT_BUILD
      _selectedServer.server = _server.servers[0];
#else
      _selectedServer.server = _server.servers[_server.servers.Length - 1];
#endif
    }
    StartCoroutine(Scripting());
  }

  IEnumerator Scripting()
  {
    yield return new WaitForSeconds(.5f);
    SceneManager.LoadScene(string.Format("_Scenes/{0}", _nextScene));
  }
}
