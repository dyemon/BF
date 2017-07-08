using UnityEngine;
using System.Collections;
using Common.Net.Http ;

public class PreloadScene : MonoBehaviour {
	public string NextScene = "Load";

	void Start() {
		SceneController.Instance.LoadScene(NextScene);
	}

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, true);
	}
}
