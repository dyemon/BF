using UnityEngine;
using System.Collections;
using Common.Net.Http;

public class FBNotLoggedScene : MonoBehaviour, IFBCallback {

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
		if(Account.Instance.IsLogged) {
			GameResources.Instance.LoadUserDataFromServer(Account.Instance.GetUserId(), OnSuccessLoadUserData, OnErrorLoadUserData);
		} else {
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Не удалось авторизоваться в FaceBook");
		}
	}

	public void OnFBLogout() {

	}


	public void OnFBLoginFail(string error) {
		if(error != null) {
			Debug.Log(error);
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Ошибка при установки соединения \n" +error);
		}
	}

	public void OnSuccessLoadUserData (HttpResponse response) {
		GameResources.Instance.MergeUserData(response.FromJson<UserData>());
		SceneController.Instance.LoadSceneAdditive("FBLogged", true);
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		SceneController.Instance.LoadSceneAdditive("FBLogged", true);
	}
}
