using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroSkillScene : WindowScene {

	public GameObjectResources GameObjectResources;
	private IList<HeroSkillData> skills;
	public static string SceneName = "HeroSkill";

	public RectTransform SkillsPanel;
	public GameObject HeroSkillButton;

	public Text MoneyAssetText;
	public Text RingAssetText;

	// Use this for initialization
	void Start () {
		if(SceneControllerHelper.instance) {
			skills = (IList<HeroSkillData>)SceneControllerHelper.instance.GetParameter(SceneName);
		} else {
			HeroSkillData[] aSkills = GameResources.Instance.GetGameData().HeroSkillData; 
			skills = new List<HeroSkillData>();
			skills.Add(aSkills[11]);
			skills.Add(aSkills[12]);
			skills.Add(aSkills[10]);
		}

		foreach(HeroSkillData skill in skills) {
			GameObject button = Instantiate(HeroSkillButton);
			button.transform.SetParent(SkillsPanel.transform);
			button.transform.localScale = new Vector3(1, 1, 1);
			button.GetComponent<HeroSkillButton>().Init(skill, OnSelectSkill);
		}

		MoneyAssetText.color = UserAssetType.Money.ToColor();
		RingAssetText.color = UserAssetType.Ring.ToColor();

		OnUpdateUserAssets(UserAssetType.Money, 0);
	}

	void OnEnable() {
		GameResources.Instance.onUpdateUserAsset += OnUpdateUserAssets;
	}

	void OnDisable() {
		GameResources.Instance.onUpdateUserAsset -= OnUpdateUserAssets;
	}

	public void OnSelectSkill(HeroSkillData skill) {
		if(!GameResources.Instance.Buy(skill)) {
			return;
		}

		Close(skill);
	}

	void OnUpdateUserAssets(UserAssetType type, int value) {
		UserData userData = GameResources.Instance.GetUserData();
		MoneyAssetText.text = userData.GetAsset(UserAssetType.Money).Value.ToString();
		RingAssetText.text = userData.GetAsset(UserAssetType.Ring).Value.ToString();
	}

	public void ShowUserAssetScene() {
		SceneController.Instance.ShowUserAssetsScene(UserAssetType.Money, false);
	}
}
