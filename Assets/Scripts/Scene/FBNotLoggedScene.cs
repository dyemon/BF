using UnityEngine;
using System.Collections;
using Common.Net.Http;

public class FBNotLoggedScene : MonoBehaviour, IFBCallback {

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
		if(Account.Instance.AccessToken != null) {
			HttpRequest request = new HttpRequest().Url(HttpRequester.URL_USER_SAVE)
				.ShowWaitPanel(true)
				.Param("userId", Account.Instance.AccessToken.UserId);

			HttpRequester.Instance.Send(request);
 
		} else {
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Не удалось авторизоваться в FaceBook");
		}
		//SceneController.Instance.LoadSceneAdditive("FBLogged", true);
	}

	public void OnFBLogout() {

	}


	public void OnFBLoginFail(string error) {
		if(error != null) {
			Debug.Log(error);
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Ошибка при установки соединения \n" +error);
		}

	}
}
