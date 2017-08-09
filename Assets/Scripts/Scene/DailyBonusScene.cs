using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusScene : WindowScene {
	public const string SceneName = "DailyBonus";

	public GameObjectResources GOResources;
	public GameObject DailyButton;
	public GameObject BonusesPanel;
	public Sprite CurrentBonusBg;
	public UserAssetsPanel AssetsPanel;
	public GameObject AwardItem;

	private bool isGetBonus = false;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
	}

	void Start() {
		UserData uData = GameResources.Instance.GetUserData();
		Preconditions.Check(!uData.DailyBonusTaken, "Daily bonus alredy taken");

		DailyBonusData dData = GameResources.Instance.GetGameData().DailyBonusData;

		int i = 0;
		foreach(AwardItem item in dData.DailyBonuses) {
			GameObject button = InitDailyBonusButton(item, ++i, uData.DailyBonus);
		}

		GameObject gButton = InitDailyBonusButton(dData.GreatestBonus, (uData.DailyBonus > i)? uData.DailyBonus : -1, uData.DailyBonus);
	}

	GameObject InitDailyBonusButton(AwardItem item, int number, int dailyBonus) {
		GameObject button = Instantiate(DailyButton, BonusesPanel.transform);

		Image img = button.transform.Find("IconBg").GetComponent<Image>();
		img.sprite = GOResources.GetIconBackground(item.Type);
		img.transform.Find("AwardType").GetComponent<Image>().sprite = GOResources.GetUserAssetIcone(item.Type);
		Text text = img.transform.Find("AwardAmount").GetComponent<Text>();
		text.text = item.Value.ToString();
		//text.color = item.Type.ToColor();

		text = button.transform.Find("DailyText").GetComponent<Text>();
		text.text = (number < 0)? "Далее..." : string.Format("День: {0}", number);

		GameObject active;
		if(number == -1 || number > dailyBonus) {
			active = button.transform.Find("NotAllow").gameObject;
		} else if(number == dailyBonus) {
			button.GetComponent<Image>().sprite = CurrentBonusBg;
			active = button.transform.Find("Allow").gameObject;
			active.transform.Find("Text Button").GetComponent<Button>()
				.onClick.AddListener(() => {OnDailyBonus(button, item);});
		} else {
			active = button.transform.Find("Taken").gameObject;
		}
		active.SetActive(true);

		return button;
	}

	public void OnDailyBonus(GameObject button, AwardItem award) {
		AssetsPanel.DisableUpdate(true);
		GameResources.Instance.ChangeUserAsset(award.Type, award.Value);
		GameResources.Instance.IncrementDailyBonus();	
		AssetsPanel.DisableUpdate(false);

		GameObject assetImg = UnityUtill.FindByName(button.transform, "AwardType").gameObject;

		GameObject animImg = Instantiate(AwardItem, transform);
		animImg.transform.position = assetImg.transform.position;
		animImg.AddComponent<AnimatedObject>();
		Vector3 end = AssetsPanel.GetUserAssetsIcon(award.Type).transform.position;
		Vector3 start = assetImg.transform.position;
		Sprite icon = GOResources.GetUserAssetIcone(award.Type);

		Animations.CreateAwardAnimation(animImg, start, end, icon, award.Value); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteTakeBonus(animImg);} ).Run();

		button.transform.Find("Allow").gameObject.SetActive(false);
		button.transform.Find("Taken").gameObject.SetActive(true);

		isGetBonus = true;
	}

	void CompleteTakeBonus(GameObject animGO) {
		Destroy(animGO);
		AssetsPanel.UpdateUserAssets();
	}

	public void OnClose() {
		if(!isGetBonus) {
			ParametersController.Instance.SetParameter(ParametersController.CANSEL_DAILYBONUS, true);
		}

		Close();
	}
}
