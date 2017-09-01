using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Animation;

public class FightHelpScene : WindowScene {
	public const string SceneName = "FightHelp";

	public GameObjectResources GOResources;

	public Text UserMoneyText;
	public Text HealthRatioText;
	public Text DamageRatioText;

	public GameObject HealthIcon;
	public GameObject DamageIcon;
	public GameObject HealthButton;
	public GameObject DamageButton;

	private HeroController heroController;

	private bool isLock = false;

	void OnEnable() {
		GameResources.Instance.onUpdateUserAsset += OnUpdateUserAssets;
	}

	void OnDisable() {
		GameResources.Instance.onUpdateUserAsset -= OnUpdateUserAssets;
	}

	// Use this for initialization
	void Start () {
		if(SceneController.Instance != null) {
			heroController = SceneController.Instance.GetParameter(SceneName) as HeroController;
		}

		UserData uData = GameResources.Instance.GetUserData();
		UserMoneyText.text = uData.GetAsset(UserAssetType.Money).Value.ToString();
		FightHelpData fd = GameResources.Instance.GetGameData().FightHelpData;

		HealthButton.transform.Find("PriceImage").GetComponent<Image>()
			.sprite = GOResources.GetUserAssetIcone(fd.PriceType);
		HealthButton.transform.Find("PriceText").GetComponent<Text>()
			.text = fd.PriceValue.ToString();
		DamageButton.transform.Find("PriceImage").GetComponent<Image>()
			.sprite = GOResources.GetUserAssetIcone(fd.PriceType);
		DamageButton.transform.Find("PriceText").GetComponent<Text>()
			.text = fd.PriceValue.ToString();

		HealthRatioText.text = "+" + fd.IncreaceHealthRatio + "%";
		DamageRatioText.text = "+" + fd.IncreaceDamageRatio + "%";
	}
	
	public void OnClickBuy(bool isHealth) {
		if(isLock) {
			return;
		}


		FightHelpData fd = GameResources.Instance.GetGameData().FightHelpData;

		if(!GameResources.Instance.ChangeUserAsset(fd.PriceType, -fd.PriceValue)) {
			SceneController.Instance.ShowUserAssetsScene(UserAssetType.Money, true);
			return;
		}
		GameResources.Instance.SaveUserData(null, false);

		isLock = true;
		Help(isHealth);
	}

	public void OnClickHalyava(bool isHealth) {
		if(isLock) {
			return;
		}
		isLock = true;

		Help(isHealth);
	}

	void OnUpdateUserAssets(UserAssetType? type, int value) {
		UserData userData = GameResources.Instance.GetUserData();
		UserMoneyText.text = userData.GetAsset(UserAssetType.Money).Value.ToString();
	}

	public void ShowUserAssetScene() {
		SceneController.Instance.ShowUserAssetsScene(UserAssetType.Money, false);
	}

	void Help(bool isHealth) {
		FightHelpData fd = GameResources.Instance.GetGameData().FightHelpData;

		if(heroController != null) {
			if(isHealth) {
				heroController.IncreaseHealth(fd.IncreaceHealthRatio, true);
			} else {
				heroController.IncreaseDamage(fd.IncreaceDamageRatio, true);
			}
		}

		GameObject animItem = Instantiate(isHealth? HealthIcon : DamageIcon , transform);
		Vector3 start = isHealth ? HealthIcon.transform.position : DamageIcon.transform.position;

		float dist = Screen.height / 2f;
		Vector3 end1 = start + new Vector3(0, dist*0.5f, 0);
		Vector3 end2 = start + new Vector3(0, dist, 0);

		float time1 = App.GetMoveTime(UIMoveType.AWARD_EXPERIENCE);
		float time2 = App.GetMoveTime(UIMoveType.AWARD_EXPERIENCE);

		AnimatedObject ao = animItem.AddComponent<AnimatedObject>();
		animItem.AddComponent<CanvasGroup>();
		ao.AddMoveByTime(start, end1, time1).Build()
			.AddMoveByTime(null, end2, time2).AddFadeUI(null, 0f, time2)
			.OnStop(() => {
			}).Build().Run();

		Destroy(animItem, 2f);
		Invoke("Close", 0.5f);
	}
}
