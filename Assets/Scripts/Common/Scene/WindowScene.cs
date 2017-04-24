using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowScene : MonoBehaviour {
		
	public void Close() {
		Close(null);
	}

	public void Close(System.Object retVal = null) {
		StartCoroutine(CloseInternal(retVal));
	}

	private IEnumerator CloseInternal(System.Object retVal) {
		FadeCanvasGroup fcg = GetComponent<FadeCanvasGroup>();
		if(fcg != null) {
			fcg.FadeOut();
			while(!fcg.IsDone) {
				yield return 0;
			}
		}

		SceneController.Instance.UnloadCurrentScene(retVal);
	}
}
