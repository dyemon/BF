using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelFailureScene : MonoBehaviour {
	public const string SceneName = "LevelFailure";

	public GameObjectResources GOResources;

	public GameObject AwardItem;
	public HorizontalLayoutGroup Awards1;
	public HorizontalLayoutGroup Awards2;
	public GameObject Experience;
	public Text Description;
	public UserAssetsPanel AssetPanel;
	public Button[] AwardButtons;


	public GameObject AwardTileItem;

	public GameObject CloseButton;
	public GameObject RepeatButton;

	public GameObject Description1;

	private int successCount = 0;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
	}

	void Start () {
		int i = 0;
		LevelData levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		UserData uData = GameResources.Instance.GetUserData();
		successCount = uData.GetSuccessCount(App.GetCurrentLevel()); 

		Description.text = App.GetCurrentLevel() + ": " + levelData.Name;

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			int collectVal = GameController.CollectLevelAward.GetAsset(type);

			if(collectVal > 0) {
				HorizontalLayoutGroup layout = (i++ > 1) ? Awards2 : Awards1;
				GameObject go = Instantiate(AwardItem, layout.transform);
				go.name = type.ToString() + "AwardItem";
				go.GetComponent<BuyButton>().Init(type, collectVal, null);
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards1.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards2.GetComponent<RectTransform>());

		Experience.GetComponent<BuyButton>().Init(null, GameController.CollectLevelAward.Experience, null, UserAssetTypeExtension.ExperienceColor);

		bool capNotEnded = ParametersController.Instance.GetBool(ParametersController.CAPITULATE_NOT_ENDED);
		ParametersController.Instance.SetParameter(ParametersController.CAPITULATE_NOT_ENDED, false);
		if(!capNotEnded) {
			ParametersController.Instance.SetParameter(ParametersController.CAN_SHOW_DAILYBONUS, true);
			ParametersController.Instance.SetParameter(ParametersController.CAN_SHOW_FORTUNA, true);

		}

		bool showAward = successCount == 0 && !capNotEnded; 

		foreach(Button b in AwardButtons) {
			b.gameObject.SetActive(showAward);
			if(showAward) {
				b.onClick.AddListener(() => {
					OnSelectAward(b.gameObject);
				});
			}
		}
		Description1.SetActive(showAward);

		RepeatButton.GetComponent<BuyButton>().Init(UserAssetType.Energy, levelData.LevelPrice, null);
		CloseButton.SetActive(!showAward);
		RepeatButton.SetActive(!showAward);
	}

	void OnSelectAward(GameObject target) {
		AwardItem award = GameResources.Instance.GetLevel(App.GetCurrentLevel()).FailureAwardData.GetRandomAwardItem();
		foreach(Button b in AwardButtons) {
			b.gameObject.SetActive(false);
		}
		Description1.SetActive(false);
		if(award == null) {
			OnCompleteAward(null);
			return;
		}
			
		Vector3 start = target.transform.position;
		Vector3 end = AssetPanel.GetUserAssetsIcon(award.Type).transform.position;
		GameObject animAward = Instantiate(AwardTileItem, transform);
		float time = Animations.CreateAwardAnimation(animAward, start, end, 
			GOResources.GetUserAssetIcone(award.Type), award.Value);
		animAward.GetComponent<AnimatedObject>().OnStop(() => {OnCompleteAward(animAward);} ).Run();

		AssetPanel.DisableUpdate(true);
		GameResources.Instance.ChangeUserAsset(award.Type, award.Value);
		AssetPanel.DisableUpdate(false);
	}

	void OnCompleteAward(GameObject goAward) {
		AssetPanel.UpdateUserAssets();

		if(goAward != null) {
			Destroy(goAward);
		}

		CloseButton.SetActive(true);
		RepeatButton.SetActive(true);
	}
}
