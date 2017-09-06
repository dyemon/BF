using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WindowScene : MonoBehaviour {
		
	public AudioClip WindowOpen;
	public AudioClip WindowClose;

	public void Close() {
		Close(null);
	}

	protected virtual void Start() {
		if(WindowOpen != null) {
			SoundController.Play(WindowOpen);
		} else {
			SoundController.Play(SoundController.Instance.WindowOpen);
		}
	}

	public void Close(System.Object retVal = null, bool playSound = true) {
		StartCoroutine(CloseInternal(retVal));
		if(playSound && WindowClose != null) {
			SoundController.Play(WindowClose);
		} else if(playSound) {
			SoundController.Play(SoundController.Instance.WindowClose);
		}
	}

	private IEnumerator CloseInternal(System.Object retVal) {
		FadeCanvasGroup fcg = GetComponent<FadeCanvasGroup>();
		if(fcg != null) {
			if(fcg != null) {
				fcg.FadeOut();
				while(!fcg.IsDone) {
					yield return 0;
				}
			}
		}

		SceneController.Instance.UnloadCurrentScene(retVal);
	}
}
