using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAssetsPanel : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	//public GameObject UserAssetsPanelItem;
	//public Transform userAssetsPanel;
	public Text ExperienceText;

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
	}

	void OnDisable() {
		GameResources.Instance.onUpdateUserAsset -= OnUpdateUserAssets;
		GameResources.Instance.onUpdateInfinityEnergy -= OnUpdateInfinityEnergy;
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
		}

		ExperienceText.text = userData.Experience.ToString();
		UpdateInfinityEnergy();
	//	StartCoroutine(AlignButtons());
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
		return GetUserAssetItem(type).gameObject;
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
			Text text = GetUserAssetItem(type).Find("Text").GetComponent<Text>();
			text.text = userData.GetAsset(type).Value.ToString();
		}

	//	LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	//	StartCoroutine(AlignButtons());
	}
		

	void OnUpdateUserAssets(UserAssetType type, int value) {
		if(disableUpdate) {
			return;
		}

		UserData userData = GameResources.Instance.GetUserData();
		Text text = GetUserAssetItem(type).Find("Text").GetComponent<Text>();
		text.text = userData.GetAsset(type).Value.ToString();
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
}
