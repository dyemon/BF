using UnityEngine;
using System.Collections;

public class FBNotLoggedScene : MonoBehaviour, IFBCallback {

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
		SceneController.Instance.LoadSceneAdditive("FBLogged", true);
	}

	public void OnFBLogout() {

	}


	public void OnFBLoginFail(string error) {
		if(error != null) {
			Debug.Log(error);
		}

	}
}
