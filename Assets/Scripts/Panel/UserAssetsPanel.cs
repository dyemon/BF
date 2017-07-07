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
	//	StartCoroutine(AlignButtons());
	}

	public GameObject GetUserAssetsIcon(UserAssetType type) {
		return GetUserAssetItem(type).gameObject;
	}

	Transform GetUserAssetItem(UserAssetType type) {
		string name = type.ToString() + "PanelItem";
		return UnityUtill.FindByName(gameObject.transform, name);
	}

	IEnumerator AlignButtons() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
	//		yield return new WaitForFixedUpdate();
		Vector3 pos = new Vector3(RightAssets.localPosition.x - RightAssets.rect.width - 30, RightAssets.localPosition.y);
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
	//	StartCoroutine(AlignButtons());
	}

	void Update() {
	//	AlignButtons();
	}
}
