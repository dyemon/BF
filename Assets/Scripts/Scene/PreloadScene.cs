using UnityEngine;
using System.Collections;

public class PreloadScene : MonoBehaviour {
	public string NextScene = "Load";

	void Start() {
		SceneController.Instance.LoadScene(NextScene);
	}
}
