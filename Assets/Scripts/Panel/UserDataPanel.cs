using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataPanel : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	public GameObject UserAssetsPanelItem;
	public Transform userAssetsPanel;
	public Text ExperienceText;

	void Start () {
		UserData userData = GameResources.Instance.GetUserData();

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			GameObject item = Instantiate(UserAssetsPanelItem);
			item.name = type.ToString() + "PanelItem";
			item.transform.SetParent(userAssetsPanel);
			item.transform.localScale = new Vector3(1,1,1);

			Image img = item.transform.Find("Image").GetComponent<Image>();
			img.sprite = GameObjectResources.GetUserAssetIcone(type);

			UserAssetData data = userData.GetAsset(type);
			Text text = item.transform.Find("Text").GetComponent<Text>();
			text.text = data.Value.ToString();
			text.color = type.ToColor();
		}

		ExperienceText.text = userData.Experience.ToString();
	}

	public GameObject GetUserAssetsIcon(UserAssetType type) {
		return userAssetsPanel.Find(type + "PanelItem/Image").gameObject;
	}

	public void UpdateUserAssets() {
		UserData userData = GameResources.Instance.GetUserData();

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			string name = type.ToString() + "PanelItem";
			Text text = userAssetsPanel.Find(name + "/Text").GetComponent<Text>();
			text.text = userData.GetAsset(type).Value.ToString();
		}
	}
}
