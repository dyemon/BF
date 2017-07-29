using UnityEngine;
using System.Collections;

public class SceneControllerHelper : Singleton<SceneController>  {

	public void LoadAsync(string name) {
		instance.LoadSceneAsync(name);
	}

	public void LoadSceneAdditive(string name) {
		LoadSceneAdditive(name, false);
	}

	public void LoadSceneAdditive(string name, bool unloadOther) {
		instance.LoadSceneAdditive(name, unloadOther);
	}

	public void UnloadScene(string name) {
		instance.UnloadScene(name);
	}

	public void LoadCurrentLevel() {
		instance.LoadCurrentLevel();
	}

	public void Quit() {
		Application.Quit();
	}

	public void LoadMainScene() {
		instance.LoadMainScene();
	}
}
