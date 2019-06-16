using System.Collections;
using UnityEngine;

public class SlowMotionMonitor : MonoBehaviour
{
	Settings _settings;
	bool _froze;

	void Awake ()
	{
		_settings = FindObjectOfType<Settings> ();
	}

	public void Freeze (float timeScale, float endTime)
	{
		if (_froze) return;
		_froze = true;
		StartCoroutine (OnFrozen (timeScale, endTime));
	}

	public void SetAsDefault ()
	{
		Time.timeScale = _settings.defaultTimeScale;
	}

	IEnumerator OnFrozen (float timeScale, float endTime)
	{
		Time.timeScale = timeScale;
		var t = 0f;
		while (t <= 1f)
		{
			t += Time.unscaledDeltaTime / endTime;
			yield return null;
		}
		Time.timeScale = _settings.defaultTimeScale;
		_froze = false;
	}
}
