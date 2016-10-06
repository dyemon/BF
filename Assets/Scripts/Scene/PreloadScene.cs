using UnityEngine;
using System.Collections;

public class PreloadScene : MonoBehaviour {
	public string nextLevel = "Start";

	void Start() {
		SceneController.Instance.LoadScene(nextLevel);
	}
}
