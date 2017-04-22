using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowScene : MonoBehaviour {

	public void Close() {
		StartCoroutine(CloseInternal());
	}

	private IEnumerator CloseInternal() {
		FadeCanvasGroup fcg = GetComponent<FadeCanvasGroup>();
		if(fcg != null) {
			fcg.FadeOut();
			while(!fcg.IsDone) {
				yield return 0;
			}
		}

		SceneController.Instance.UnloadCurrentScene();
	}
}
