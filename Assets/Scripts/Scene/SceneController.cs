using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour {
	public static SceneController Instance;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	public void LoadScene(string name) {
		SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
	}
		
}
