using UnityEngine;
using System.Collections;

public class MapScene : MonoBehaviour {
	public const string SceneName = "Map";

	void Start () {
		Invoke("ShowAdditionScenes", 2);
	}
	
	public void OnClickFB() {
		if(!Account.Instance.IsLogged) {
			SceneController.Instance.LoadSceneAdditive("FBNotLogged");
		} else {
			SceneController.Instance.LoadSceneAdditive("FBLogged");
		}
	}

	void ShowAdditionScenes() {
		UserData uData = GameResources.Instance.GetUserData();

		bool canShowDailyBonus = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_DAILYBONUS);
		bool dailyBonusIsShown = ParametersController.Instance.GetBool(ParametersController.DAILYBONUS_IS_SHOWN);

		if(!uData.DailyBonusTaken && canShowDailyBonus && !dailyBonusIsShown) {
			SceneController.Instance.LoadSceneAdditive(DailyBonusScene.SceneName);
		}
	}
}
