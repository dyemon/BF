using UnityEngine;
using System.Collections;

public class FBLoggedScene : MonoBehaviour, IFBCallback {

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
