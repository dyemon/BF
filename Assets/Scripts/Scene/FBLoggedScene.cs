using UnityEngine;
using System.Collections;

public class FBLoggedScene : WindowScene, IFBCallback {
	public const string SceneName = "FBLogged";

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {

	}

	public void OnFBLogout() {
		SceneController.Instance.UnloadScene("FBLogged");

	}


	public void OnFBLoginFail(string error) {

	}
}
