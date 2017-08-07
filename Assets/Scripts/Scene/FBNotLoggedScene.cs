using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Collections.Generic;

public class FBNotLoggedScene : WindowScene, IFBCallback {
	public const string SceneName = "FBNotLogged";

	public const string QUEST_ID_FB_AUTHORIZE = "authorizeFB";

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
		if(Account.Instance.IsLogged) {
			QuestProgressData quest = GameResources.Instance.GetUserData().GetQuestById(QUEST_ID_FB_AUTHORIZE);
			if(quest != null && !quest.IsComplete) {
				GameResources.Instance.IncreasQuestAction(quest.QuestId, 1, true);
			}

			GameResources.Instance.LoadUserDataFromServer(Account.Instance.GetUserId(), true, OnSuccessLoadUserData, OnErrorLoadUserData);

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
		GameResources.Instance.MergeUserData(response.FromJson<UserData>());
		SceneController.Instance.LoadSceneAdditive("FBLogged", null, true);
	}

	public void OnErrorLoadUserData (HttpResponse response) {
		SceneController.Instance.LoadSceneAdditive("FBLogged", null, true);
	}
}
