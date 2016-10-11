using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class SceneController : MonoBehaviour {
	public static SceneController Instance;

//	public Image fadeImage;
	public float MinDuration = 0.05f;
	public float MaxAlpha = 0.5f;
	public float ChangeTimeSeconds = 0.05f;

	private float startAlpha = 0;
	private float endAlpha = 1;
	private Vector3 loadRotateAngles = new Vector3(0, 0, -10);

	float changeRate = 0;
	float timeSoFar = 0;
	bool fading = false;
	CanvasGroup canvasGroup;

	public Image LoadImage;

	private IList<string> loaddedScenes = new List<string>();

	void Awake() {
		Instance = this;
	}

	void Start() {
		canvasGroup = GetComponent<CanvasGroup>();
		if(canvasGroup == null) {
			Debug.Log("Must have canvas group attached!");
			this.enabled = false;
		} else {
			canvasGroup.alpha = 0f;
		}
	}

	public void LoadScene(string name) {
		loaddedScenes.Clear();
		SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
	}

	public void LoadSceneAdditive(string name) {
		if(!loaddedScenes.Contains(name)) {
			SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
			loaddedScenes.Add(name);
		}
	}

	public void UnloadScene(string name) {
		SceneManager.UnloadScene(name);
		loaddedScenes.Remove(name);
	}

	public void LoadSceneAsync(string name) {
		loaddedScenes.Clear();
		StartCoroutine(LoadSceneAsyncInternal(name));
	}

	void Update() {
	/*	if(Input.GetMouseButtonDown(0)) {
			LoadSceneAsync("Start");
		}
		*/

		if(LoadImage != null && canvasGroup.alpha > 0) {
			LoadImage.transform.Rotate(loadRotateAngles);
		}
	}

	private IEnumerator LoadSceneAsyncInternal(string sceneName) {
		FadeIn();

		float endTime = Time.time + MinDuration;
		while(Time.time < endTime)
			yield return null;

		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}

	void OnLevelWasLoaded() {
		if(canvasGroup.alpha > 0) {
			FadeOut();
		}
	}
		
	public void FadeIn() {
		startAlpha = canvasGroup.alpha;
		endAlpha = MaxAlpha;
		timeSoFar = 0;
		fading = true;
		StartCoroutine(FadeCoroutine());
	}

	public void FadeOut() {
		startAlpha = canvasGroup.alpha;
		endAlpha = 0;
		timeSoFar = 0;
		fading = true;
		StartCoroutine(FadeCoroutine());
	}

	IEnumerator FadeCoroutine() {
		changeRate = (endAlpha - startAlpha) / ChangeTimeSeconds;
		SetAlpha(startAlpha);
		while(fading) {
			timeSoFar += Time.deltaTime;

			if(timeSoFar > ChangeTimeSeconds) {
				fading = false;
				SetAlpha(endAlpha);
				yield break;
			} else {
				SetAlpha(canvasGroup.alpha + (changeRate * Time.deltaTime));
			}

			yield return null;
		}
	}
		
	public void SetAlpha(float alpha) {
		canvasGroup.alpha = Mathf.Clamp(alpha, 0, 1);
	}
}
