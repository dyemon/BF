using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Collections.Generic;

public class FBNotLoggedScene : WindowScene, IFBCallback {
	public const string SceneName = "FBNotLogged";

	public const string QUEST_ID_FB_AUTHORIZE = "authorizeFB";

	void OnEnable() {
		HttpRequester.Instance.AddEventListener(HttpRequester.URL_USER_LOAD, OnSuccessLoadUserData, OnErrorLoadUserData);
	}

	void OnDisable() {
		HttpRequester.Instance.RemoveEventListener(HttpRequester.URL_USER_LOAD, OnSuccessLoadUserData, OnErrorLoadUserData);

	}

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
		if(Account.Instance.IsLogged) {
			QuestProgressData quest = GameResources.Instance.GetUserData().GetQuestById(QUEST_ID_FB_AUTHORIZE);
			if(quest != null && !quest.IsComplete) {
				GameResources.Instance.IncreasQuestAction(quest.QuestId, 1, true);
			}

			GameResources.Instance.LoadUserDataFromServer(true);

		} else {
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Не удалось авторизоваться в facebook");
		}
	}

	public void OnFBLogout() {
	}
	public void OnFriendsRequest(IList<FBUser> friends) {
	}

	public void OnFBLoginFail(string error) {
		if(error != null) {
			Debug.Log(error);
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Ошибка при установки соединения \n" +error);
		}
	}

	public void OnSuccessLoadUserData (HttpResponse response) {
		try {
			UserData uData = response.GetData<UserData>();
			GameResources.Instance.CheckGift();
			if(GameResources.Instance.MergeUserData(uData)) {
				ModalPanels.Show(ModalPanelName.MessagePanel,
					string.Format("Данные обновлены. Выш текущий уровень {0}", uData.Level));
				SceneController.Instance.LoadSceneAsync(LocationScene.SceneName);
				return;
			}
		} catch (System.Exception e) {
			Debug.LogError(e);
		}

		SceneController.Instance.LoadSceneAdditive("FBLogged", null, true);
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		if(response.IsUserNotFound()) {
			GameResources.Instance.SaveUserData(null, true);
		}
		SceneController.Instance.LoadSceneAdditive("FBLogged", null, true);
	}
}
