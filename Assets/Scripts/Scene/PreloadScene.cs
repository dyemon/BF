using UnityEngine;
using System.Collections;

public class PreloadScene : MonoBehaviour {
	public string StartScene = "Start";

	void Start() {
		SceneController.Instance.LoadScene(StartScene);
	}
}
