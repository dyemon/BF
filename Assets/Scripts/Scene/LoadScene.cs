using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour, IFBCallback {
	public string NextScene = "Maps";


	public void OnFBInit() {
		SceneController.Instance.LoadScene(NextScene);	
	}

	public void OnFBLoginSuccess() {
		
	}

	public void OnFBLogout() {
		
	}

	public void OnFBLoginFail(string error) {
		
	}
}
