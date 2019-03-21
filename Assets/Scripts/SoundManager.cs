using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public Sound[] sounds;

	void Awake ()
	{
		foreach (var sound in sounds)
		{
			var source = gameObject.AddComponent<AudioSource> ();
			sound.source = source;
			source.clip = sound.clip;
			source.volume = sound.volume;
			source.pitch = sound.pitch;
			source.loop = sound.loop;
		}
	}

	public void Play (string name)
	{
		var found = System.Array.Find (sounds, sound => sound.name == name);
		if (found == null) return;
		found.source.Play ();
	}

	public void SetVolume (string name, float volume)
	{
		var found = System.Array.Find (sounds, sound => sound.name == name);
		if (found == null) return;
		found.source.volume = volume;
	}
}
