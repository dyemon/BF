using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour {
	private bool enableAdditiveScene = true;

	void ShowAdditionScenes() {
		if(!enableAdditiveScene) {
			return;
		}

		if(SceneController.Instance.HasLoadedAdditiveScene()) {
			Invoke("ShowAdditionScenes", 3);
			return;
		}

		UserData uData = GameResources.Instance.GetUserData();

		bool canShow = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_DAILYBONUS);
		bool isCansel = ParametersController.Instance.GetBool(ParametersController.CANSEL_DAILYBONUS);

		if(!uData.DailyBonusTaken && canShow && !isCansel) {
			SceneController.Instance.LoadSceneAdditive(DailyBonusScene.SceneName);
		//	Invoke("ShowAdditionScenes", 3);
			return;
		}

		canShow = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_FORTUNA);
		 bool isShown = ParametersController.Instance.GetBool(ParametersController.FORTUNA_IS_SHOWN);

		if(uData.FortunaTryCount > 0 && canShow && !isShown) {
			SceneController.Instance.LoadSceneAdditive(FortunaScene.SceneName);
			return;
		}

		canShow = ParametersController.Instance.GetBool(ParametersController.CAN_SHOW_BLATHATA);
		isShown = ParametersController.Instance.GetBool(ParametersController.BLATHATA_IS_SHOWN);

		if(uData.GetAsset(UserAssetType.Star).Value > 0 && canShow && !isShown) {
			SceneController.Instance.LoadSceneAdditive(BlathataScene.SceneName);
			return;
		}
	}

	public void DisableAdditiveScenes() {
		enableAdditiveScene = false;
	}
}
