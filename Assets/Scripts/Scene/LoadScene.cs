using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Collections.Generic;

public class LoadScene : MonoBehaviour, IFBCallback {

	void Awake() {
		ModalPanels.Init();
		GameResources.Instance.LoadSettings();

		HttpRequester.Instance.SetBaseUrl(GameResources.Instance.Settings.ReadValue("net", "baseUrl", ""));
	}



	public void OnFBInit() {
		if(Account.Instance.IsLogged) {
			GameResources.Instance.LoadUserDataFromServer(Account.Instance.GetUserId(), false, OnSuccessLoadUserData, OnErrorLoadUserData);
		} else {
			SceneController.Instance.LoadMainScene();
		}
	}

	public void OnFBLoginSuccess() {
		
	}

	public void OnFBLogout() {
		
	}

	public void OnFBLoginFail(string error) {
		
	}
	public void OnFriendsRequest(IList<FBUser> friends) {
	}

	public void OnSuccessLoadUserData (HttpResponse response) {
		GameResources.Instance.MergeUserData(response.FromJson<UserData>());
		SceneController.Instance.LoadMainScene();
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		SceneController.Instance.LoadMainScene();	
	}
}
