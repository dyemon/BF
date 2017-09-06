using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	public static MusicController Instance;

	private bool enable;

	public AudioClip Default;
	public AudioClip GameScene;

	private AudioClip currentClip;
	private AudioSource source;

	void Awake() {
		Instance = this;
		enable = GameResources.Instance.GetLocalData().MusicOn;
	}

	void Start() {
		currentClip = Default;
		source = GetComponent<AudioSource>();
	}

	public void Enable(bool val) {
		enable = val;
		if(val) {
			source.clip = currentClip;
			source.Play();
		} else {
			source.Stop();
		}
	}

	public void PlayClip(AudioClip clip) {
		currentClip = clip;
		if(!enable) {
			return;
		}

		source.clip = clip;
		source.Play();
	}

	public static void Play(AudioClip clip) {
		Instance.PlayClip(clip);
	}
}
