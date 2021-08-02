using UnityEngine;

public class Settings : MonoBehaviour
{
	public float playerFootOnGroundDelta = 2f;
	public float defaultTimeScale;
	public float defaultFendingOffStatusOffTime = .1f;
	public float defaultReleaseLockExplosionTime = .25f;

	/// <summary>
  /// [Readonly] Returns true if this object is active on an active server.
  /// </summary>
  public bool isServer { get; private set; }

  /// <summary>
  /// [Readonly] Returns true if running as a client and this object was spawned by a server.
  /// </summary>
  public bool isClient { get; private set; }

  static Settings pInstance;

  /// <summary>
  /// Get the Settings through a static instance
  /// </summary>
  /// <value></value>
  public static Settings instance
  {
    get
    {
      return pInstance ?? (pInstance = FindObjectOfType<Settings> ());
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake ()
  {
    // If application runs in batch-mode.
    if (Application.isBatchMode)
    {
      Debug.Log ("Run as server.");
      isServer = true;
      isClient = false;
      return;
    }
    // If application runs on WebGL/Editor.
    Debug.Log ("Run as client.");
    isServer = false;
    isClient = true;
  }
}
