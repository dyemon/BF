using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour, IFBCallback {
	public string NextScene = "Maps";

	void Start() {
		ModalPanels.Init();

		GameResources.LoadSettings();
		GameResources.LoadUserData();

		HttpRequester.Instance.SetBaseUrl(GameResources.Settings.ReadValue("net", "baseUrl", ""));
	}

	public void OnFBInit() {
		if(Account.Instance.AccessToken != null) {
			GameResources.LoadUserDataFromServer(false, true);
		} else {
			SceneController.Instance.LoadScene(NextScene);	
		}
	}

	public void OnFBLoginSuccess() {
		
	}

	public void OnFBLogout() {
		
	}

	public void OnFBLoginFail(string error) {
		
	}
}
