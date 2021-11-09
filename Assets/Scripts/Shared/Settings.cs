using UnityEngine;

public class Settings : MonoBehaviour
{
  public float playerFootOnGroundDelta = 2f;
  public float defaultTimeScale;
  public float defaultFendingOffStatusOffTime = .1f;
  public float defaultReleaseLockExplosionTime = .25f;

  SelectedServer _selectedServer;
  public SelectedServer selectedServer { get => _selectedServer ?? (_selectedServer = FindObjectOfType<SelectedServer>()); }

  /// <summary>
  /// [Readonly] Returns true if this object is active on an active server.
  /// </summary>
  public bool isServer { get => Application.isBatchMode; }

  /// <summary>
  /// [Readonly] Returns true if running as a client and this object was spawned by a server.
  /// </summary>
  public bool isClient { get => !Application.isBatchMode; }

  static Settings _instance;

  /// <summary>
  /// Get the Settings through a static instance
  /// </summary>
  /// <value></value>
  public static Settings instance
  {
    get
    {
      return _instance ?? (_instance = FindObjectOfType<Settings>());
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake()
  {
    // If application runs in batch-mode.
    if (Application.isBatchMode)
    {
      print("Run as server.");
      Application.targetFrameRate = 60;
      return;
    }
    // If application runs on WebGL/Editor.
    print("Run as client.");
  }
}
