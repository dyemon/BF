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

	private EducationController educationController;

	// Use this for initialization
	protected override void Start() {
		base.Start();

		if(SceneControllerHelper.instance != null) {
			skills = (IList<HeroSkillData>)SceneControllerHelper.instance.GetParameter(SceneName);
		} else {
			HeroSkillData[] aSkills = GameResources.Instance.GetGameData().HeroSkillData; 
			skills = new List<HeroSkillData>();
			skills.Add(aSkills[11]);
			skills.Add(aSkills[12]);
			skills.Add(aSkills[10]);
		}
		int i = 1;
		foreach(HeroSkillData skill in skills) {
			GameObject button = Instantiate(HeroSkillButton);
			button.transform.SetParent(SkillsPanel.transform);
			button.name = "HeroSkillButton" + i++;
			button.transform.localScale = new Vector3(1, 1, 1);
			button.GetComponent<HeroSkillButton>().Init(skill, OnSelectSkill);
		}

		MoneyAssetText.color = UserAssetType.Money.ToColor();
		RingAssetText.color = UserAssetType.Ring.ToColor();

		OnUpdateUserAssets(UserAssetType.Money, 0);

		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		if(levelData.HasEducation()) {
			GameObject Education = GameObject.Find("Education"); 
			if(Education != null) {
				educationController = Education.GetComponent<EducationController>();
				if(educationController.IsCurrentEducationType(EducationType.UseHeroSkill)) {
					Education.transform.SetParent(transform);
					educationController.Next();
					Invoke("StartEducation", 0.2f);

				}
			}
		}

	}

	void OnEnable() {
		GameResources.Instance.onUpdateUserAsset += OnUpdateUserAssets;
	}

	void OnDisable() {
		GameResources.Instance.onUpdateUserAsset -= OnUpdateUserAssets;
	}

	public void OnSelectSkill(HeroSkillData skill) {
		if(educationController != null && educationController.IsCurrentEducationType(EducationType.UseHeroSkill)) {
			educationController.Next();
			educationController.StartStep();
		}
			
		if(!GameResources.Instance.Buy(skill)) {
			return;
		}

		Close(skill, false);
	}

	void OnUpdateUserAssets(UserAssetType? type, int value) {
		UserData userData = GameResources.Instance.GetUserData();
		MoneyAssetText.text = userData.GetAsset(UserAssetType.Money).Value.ToString();
		RingAssetText.text = userData.GetAsset(UserAssetType.Ring).Value.ToString();
	}

	public void ShowUserAssetScene() {
		SceneController.Instance.ShowUserAssetsScene(UserAssetType.Money, false);
	}

	void StartEducation() {
		educationController.StartStep();
	}
}
