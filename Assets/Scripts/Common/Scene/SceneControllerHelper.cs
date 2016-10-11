using UnityEngine;
using System.Collections;

public class SceneControllerHelper : Singleton<SceneController>  {

	public void LoadAsync(string name) {
		instance.LoadSceneAsync(name);
	}

	public void LoadSceneAdditive(string name) {
		instance.LoadSceneAdditive(name);
	}

	public void UnloadScene(string name) {
		instance.UnloadScene(name);
	}

	public void Quit() {
		Application.Quit();
	}
}
