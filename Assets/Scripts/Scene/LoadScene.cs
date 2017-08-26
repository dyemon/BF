using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Collections.Generic;

public class LoadScene : MonoBehaviour, IFBCallback {


	void Awake() {
		ModalPanels.Init();
		GameResources.Instance.LoadSettings();
		GameResources.Instance.AddEventListeners();

		HttpRequester.Instance.SetBaseUrl(GameResources.Instance.Settings.ReadValue("net", "baseUrl", ""));
	}

	void OnEnable() {
		HttpRequester.Instance.AddEventListener(HttpRequester.URL_USER_LOAD, OnSuccessLoadUserData, OnErrorLoadUserData);
	}

	void OnDisable() {
		HttpRequester.Instance.RemoveEventListener(HttpRequester.URL_USER_LOAD, OnSuccessLoadUserData, OnErrorLoadUserData);
	}

	public void OnFBInit() {
		if(Account.Instance.IsLogged) {
			GameResources.Instance.LoadUserDataFromServer(false);
		} else {
			SceneController.Instance.LoadMainScene();
		}
	}

	public void OnFBLoginSuccess() {
		
	}

	public void OnFBLogout() {
		
	}

	public void OnFBLoginFail(string error) {
	//	if(error != null) {
	//		Debug.Log(error);
	//		ModalPanels.Show(ModalPanelName.ErrorPanel, "Ошибка при установки соединения \n" +error);
	//	}
		SceneController.Instance.LoadMainScene();
	}
	public void OnFriendsRequest(IList<FBUser> friends) {
	}

	public void OnSuccessLoadUserData (HttpResponse response) {
		try {
			UserData uData = response.GetData<UserData>();
			GameResources.Instance.CheckGift();
			if(GameResources.Instance.MergeUserData(uData)) {
				ModalPanels.Show(ModalPanelName.MessagePanel,
					string.Format("Данные обновлены. Выш текущий уровень {0}", uData.Level));	
				SceneController.Instance.LoadSceneAsync(LocationScene.SceneName);
			}
		} catch (System.Exception e) {
			Debug.LogError(e);
		}

		SceneController.Instance.LoadMainScene();
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		SceneController.Instance.LoadMainScene();	
	}
}
