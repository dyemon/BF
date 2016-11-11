using UnityEngine;
using System.Collections;
using Common.Net.Http;

public class LoadScene : MonoBehaviour, IFBCallback {
	public string NextScene = "Maps";

	void Awake() {
		ModalPanels.Init();

		GameResources.Instance.LoadSettings();
		GameResources.Instance.LoadUserData();

		HttpRequester.Instance.SetBaseUrl(GameResources.Instance.Settings.ReadValue("net", "baseUrl", ""));
	}

	public void OnFBInit() {
		if(Account.Instance.IsLogged) {
			GameResources.Instance.LoadUserDataFromServer(Account.Instance.GetUserId(), false, OnSuccessLoadUserData, OnErrorLoadUserData);
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

	public void OnSuccessLoadUserData (HttpResponse response) {
		GameResources.Instance.MergeUserData(response.FromJson<UserData>());
		SceneController.Instance.LoadScene(NextScene);	
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		SceneController.Instance.LoadScene(NextScene);	
	}
}
