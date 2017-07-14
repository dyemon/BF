using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAssetsPanel : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	//public GameObject UserAssetsPanelItem;
	//public Transform userAssetsPanel;
	public GameObject Experience;

	public RectTransform LeftAssets;
	public RectTransform LeftButton;
	public RectTransform RightAssets;
	public RectTransform RightButton;

	private string currentSceneName;
	private WindowScene currentScene;
	public GameObject InfinityEnergy;

	private bool disableUpdate = false;

	void OnEnable() {
		GameResources.Instance.onUpdateUserAsset += OnUpdateUserAssets;
		GameResources.Instance.onUpdateInfinityEnergy += OnUpdateInfinityEnergy;
		GameResources.Instance.onUpdateExperience += OnUpdateExperience;
	}

	void OnDisable() {
		GameResources.Instance.onUpdateUserAsset -= OnUpdateUserAssets;
		GameResources.Instance.onUpdateInfinityEnergy -= OnUpdateInfinityEnergy;
		GameResources.Instance.onUpdateExperience -= OnUpdateExperience;
	}

	void Start () {
		UserData userData = GameResources.Instance.GetUserData();

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			GameObject item = GetUserAssetItem(type).gameObject;
			Image img = item.transform.Find("Image").GetComponent<Image>();
			img.sprite = GameObjectResources.GetUserAssetIcone(type);

			UserAssetData data = userData.GetAsset(type);
			Text text = item.transform.Find("Text").GetComponent<Text>();
			text.text = data.Value.ToString();
			text.color = type.ToColor();
			LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>());
		}

		UpdateExperience(userData.Experience);

		UpdateInfinityEnergy();
	//	StartCoroutine(AlignButtons());
	}

	public void UpdateExperience(int exp) {
		Text ExperienceText = Experience.transform.Find("Text").GetComponent<Text>();
		ExperienceText.text = exp.ToString();
		ExperienceText.color = UserAssetTypeExtension.ExperienceColor;
		LayoutRebuilder.ForceRebuildLayoutImmediate(Experience.GetComponent<RectTransform>());
	}

	public void SetCurrentScene(string name, WindowScene scene) {
		currentSceneName = name;
		currentScene = scene;

		if(name == LombardScene.SceneName && RightButton != null) {
			RightButton.GetComponent<Button>().interactable = false;
		}else if(name == EnergyScene.SceneName && LeftButton != null) {
			LeftButton.GetComponent<Button>().interactable = false;
		}
	}

	public void DisableUpdate(bool val) {
		disableUpdate = val;
	}

	public GameObject GetUserAssetsIcon(UserAssetType type) {
		return GetUserAssetItem(type).Find("Image").gameObject;
	}
	public GameObject GetInfinityEnergyObject() {
		return InfinityEnergy;
	}

	Transform GetUserAssetItem(UserAssetType type) {
		string name = type.ToString() + "PanelItem";
		return UnityUtill.FindByName(gameObject.transform, name);
	}

	void AlignButtons() {
		Vector3 pos = new Vector3(RightAssets.localPosition.x - RightAssets.rect.width, RightAssets.localPosition.y);
		RightButton.transform.localPosition = pos;
		pos = new Vector3(LeftAssets.localPosition.x + LeftAssets.rect.width + 30, LeftAssets.localPosition.y);
	//	Debug.Log(LeftAssets.rect.width);
		LeftButton.transform.localPosition = pos;

	}
		

	public void UpdateUserAssets() {
		UserData userData = GameResources.Instance.GetUserData();

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			UpdateUserAsset(type, userData.GetAsset(type).Value);
		}

	//	LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	//	StartCoroutine(AlignButtons());
	}
		
	void UpdateUserAsset( UserAssetType type, int value) {
		Transform item = GetUserAssetItem(type);
		Text text = item.Find("Text").GetComponent<Text>();
		text.text = value.ToString();
		LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>());
	}

	void OnUpdateUserAssets(UserAssetType type, int value) {
		if(disableUpdate) {
			return;
		}

		UpdateUserAsset(type, value);
	}

	public bool UpdateInfinityEnergy() {
		UserData uData = GameResources.Instance.GetUserData();

		if(uData.InfinityEnergyDuration > 0) {
			InfinityEnergy.SetActive(true);
			Text time = InfinityEnergy.transform.Find("Text").GetComponent<Text>();
			time.text = DateTimeUtill.FormatMinutes(uData.InfinityEnergyDuration);
			return true;
		} else {
			InfinityEnergy.SetActive(false);
			return false;
		}
	}

	public void OnUpdateInfinityEnergy(int value) {
		if(disableUpdate) {
			return;
		}

		UserData userData = GameResources.Instance.GetUserData();
		Text text = GetUserAssetItem(UserAssetType.Money).Find("Text").GetComponent<Text>();
		text.text = userData.GetAsset(UserAssetType.Money).Value.ToString();

		UpdateInfinityEnergy();
	}

	public void OnLombardClick() {
		if(!string.IsNullOrEmpty(currentSceneName) && currentScene != null) {
			if(currentSceneName == EnergyScene.SceneName && SceneController.Instance.IsSceneEist(LombardScene.SceneName)) {
				currentScene.Close();
				return;
			}
		}

		SceneController.Instance.LoadSceneAdditive(LombardScene.SceneName);
	}

	public void OnEnergyClick() {
		if(!string.IsNullOrEmpty(currentSceneName) && currentScene != null) {
			if(currentSceneName == LombardScene.SceneName && SceneController.Instance.IsSceneEist(EnergyScene.SceneName)) {
				currentScene.Close();
				return;
			}
		}

		SceneController.Instance.LoadSceneAdditive(EnergyScene.SceneName);
	}

	public void OnUpdateExperience(int exp) {
		if(disableUpdate) {
			return;
		}
			
		UpdateExperience(exp);
	}

	public void UpdateExperience() {
		UserData uData = GameResources.Instance.GetUserData();
		UpdateExperience(uData.Experience);
	}
}
