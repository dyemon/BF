using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAssetsPanel : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	public GameObject UserAssetsPanelItem;
	public string tag;
	public Transform userAssetsPanel;

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
	}

	public GameObject GetUserAssetsIcon(UserAssetType type) {
		return userAssetsPanel.Find(type + "PanelItem/Image").gameObject;
	}
}
