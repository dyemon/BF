using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Animation;

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

	private IList<GameObject> awardItems = new List<GameObject>();

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
	}

	void Start () {
		int i = 0;
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		UserData uData = GameResources.Instance.GetUserData();
		successCount = uData.GetSuccessCount(App.CurrentLevel); 

		Description.text = App.CurrentLevel + ": " + levelData.Name;

		AssetPanel.DisableUpdate(true);
		bool hasAward = false;

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			int collectVal = GameController.CollectLevelAward.GetAsset(type);

			if(collectVal > 0) {
				HorizontalLayoutGroup layout = (i++ > 1) ? Awards2 : Awards1;
				GameObject go = Instantiate(AwardItem, layout.transform);
				go.name = type.ToString() + "AwardItem";
				go.GetComponent<BuyButton>().Init(type, collectVal, null);

				GameResources.Instance.ChangeUserAsset(type, collectVal);
				awardItems.Add(go);
				hasAward = true;
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards1.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards2.GetComponent<RectTransform>());

		Experience.GetComponent<BuyButton>().Init(null, GameController.CollectLevelAward.Experience, null, UserAssetTypeExtension.ExperienceColor);
		if(GameController.CollectLevelAward.Experience > 0) {
			awardItems.Add(Experience);
			hasAward = true;
			GameResources.Instance.IncreaseExperience(GameController.CollectLevelAward.Experience);
		}

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
		CloseButton.SetActive(!showAward && !hasAward);
		RepeatButton.SetActive(!showAward && !hasAward);
		AssetPanel.DisableUpdate(false);

		if(!showAward) {
			Invoke("StartAwardAnimate", 1);
		}
	}

	void OnSelectAward(GameObject target) {
		AwardItem award = GameResources.Instance.GetLevel(App.CurrentLevel).FailureAwardData.GetRandomAwardItem();
		foreach(Button b in AwardButtons) {
			b.gameObject.SetActive(false);
		}
		Description1.SetActive(false);
		if(award == null) {
			StartAwardAnimate();
			return;
		}

		AnimationGroup ag = GetComponent<AnimationGroup>();

		Vector3 start = target.transform.position;
		Vector3 end = AssetPanel.GetUserAssetsIcon(award.Type).transform.position;
		GameObject animAward = Instantiate(AwardTileItem, transform);
		float time = Animations.CreateAwardAnimation(animAward, start, end, 
			GOResources.GetUserAssetIcone(award.Type), award.Value);
		ag.Add(animAward.GetComponent<AnimatedObject>());
			
		animAward.GetComponent<AnimatedObject>().OnStop(() => {OnCompleteAward(animAward);} );

		AssetPanel.DisableUpdate(true);
		GameResources.Instance.ChangeUserAsset(award.Type, award.Value);
		AssetPanel.DisableUpdate(false);

		StartAwardAnimate();
	}

	void StartAwardAnimate() {
		AnimationGroup ag = GetComponent<AnimationGroup>();

		foreach(GameObject item in awardItems) {
			PriceItem pi = item.GetComponent<BuyButton>().GetPriceItem();
			Sprite icon = pi == null ? GOResources.GetUserExperienceIcone() : GOResources.GetUserAssetIcone(pi.Type);
			int amount = pi == null ? GameController.CollectLevelAward.Experience : pi.Value;
			Vector3 start = item.transform.position;
			GameObject targetGO = pi == null ? AssetPanel.GetExperienceIcon() : AssetPanel.GetUserAssetsIcon(pi.Type);
			Vector3 end = targetGO.transform.position;
			Vector3? endSize = pi == null? new Vector3(0.7f, 0.7f, 1) : (Vector3?)null; 

			GameObject animAward = Instantiate(AwardTileItem, transform);
			Animations.CreateAwardAnimation(animAward, start, end, icon, amount, endSize);
			animAward.GetComponent<AnimatedObject>().OnStop(() => {OnCompleteAward(animAward);} );

			ag.Add(animAward.GetComponent<AnimatedObject>());
		}

		if(ag.AnimationExist()) {
			ag.Run<GameObject>(OnCompleteAwardAnimate, (GameObject)null);
		} else {
			OnCompleteAwardAnimate(null);
		}
	}

	void OnCompleteAward(GameObject goAward) {
	//	AssetPanel.UpdateUserAssets();

		if(goAward != null) {
			Destroy(goAward);
		}

	//	CloseButton.SetActive(true);
	//	RepeatButton.SetActive(true);
	}

	void OnCompleteAwardAnimate(GameObject goAward) {
		AssetPanel.UpdateUserAssets();
		AssetPanel.UpdateExperience();

		CloseButton.SetActive(true);
		RepeatButton.SetActive(true);
	}


}
