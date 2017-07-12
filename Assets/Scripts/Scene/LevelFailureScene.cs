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

		bool showAward = successCount == 0; 

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
		Vector3 direction = (end - start).normalized;
		float dist = Vector3.Distance(start, end);
		Vector3 end1 = start + direction * dist * 0.1f;

		GameObject animAward = Instantiate(AwardTileItem, transform);
		Image img = animAward.transform.Find("Image").gameObject.GetComponent<Image>();
		img.sprite = GOResources.GetUserAssetIcone(award.Type);
		Text text = animAward.transform.Find("Text").gameObject.GetComponent<Text>();
		text.text = award.Value.ToString();
		//	text.color = award.Type.ToColor();

		AnimatedObject ao = animAward.GetComponent<AnimatedObject>();
		float time1 = App.GetMoveTime(UIMoveType.FAILURE_AWARD_1);
		float time2 = App.GetMoveTime(UIMoveType.FAILURE_AWARD_2);

		ao.AddMoveByTime(start, end1, time1).AddResize(null, new Vector3(1.5f, 1.5f, 1f), time1).Build()
			.AddIdle(0.05f).Build()
			.AddMoveByTime(null, end, time2).AddResize(null, new Vector3(1f, 1f, 1f), time2)
			.OnStop(() => {OnCompleteAward(animAward);} ).Build()
			.Run();

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
