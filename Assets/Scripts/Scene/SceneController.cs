using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class SceneController : MonoBehaviour {
	public static SceneController Instance;

	public Image fadeImage;
	public float MinDuration = 1.5f;
	public float fadeSpeed = 0.5f;

	private IList<string> loaddedScenes = new List<string>();

	void Awake() {
		Instance = this;
	}
	void Start() {
		fadeImage.canvasRenderer.SetAlpha (0f);
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
	/*
	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			LoadSceneAsync("Start");
		}
	}
	*/
	private IEnumerator LoadSceneAsyncInternal(string sceneName)
	{
		fadeImage.CrossFadeAlpha(0.5f, fadeSpeed ,false);
	
		float endTime = Time.time + MinDuration;
		while (Time.time < endTime)
			yield return null;

		AsyncOperation aop = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}

	void OnLevelWasLoaded() {
		fadeImage.CrossFadeAlpha(0.0f, fadeSpeed, false);
	} 



}
