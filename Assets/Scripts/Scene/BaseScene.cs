using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour {

	void ShowAdditionScenes() {
		if(SceneController.Instance.HasLoadedAdditiveScene()) {
			Invoke("ShowAdditionScenes", 3);
			return;
		}

		UserData uData = GameResources.Instance.GetUserData();

		bool canShow = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_DAILYBONUS);
		bool isShown = ParametersController.Instance.GetBool(ParametersController.DAILYBONUS_IS_SHOWN);

		if(!uData.DailyBonusTaken && canShow && !isShown) {
			SceneController.Instance.LoadSceneAdditive(DailyBonusScene.SceneName);
			Invoke("ShowAdditionScenes", 3);
			return;
		}

		canShow = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_FORTUNA);
		isShown = ParametersController.Instance.GetBool(ParametersController.FORTUNA_IS_SHOWN);

		if(uData.FortunaTryCount > 0 && canShow && !isShown) {
			SceneController.Instance.LoadSceneAdditive(FortunaScene.SceneName);
			return;
		}
	}
}
