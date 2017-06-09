using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LombardScene : WindowScene {
	public static string SceneName = "Lombard";

	public RectTransform Offers;
	public string BuyButtonTag;
	public ToggleGroup UserAssetTypeToggleGroup;

	public GameObject[] BuyButtons;

	private IDictionary<UserAssetType, GameObject> buttons = new Dictionary<UserAssetType, GameObject>();
	private IDictionary<string, UserAssetType> toggleNames = new Dictionary<string, UserAssetType>();

	private UserAssetsShopData shopData;
	private UserAssetType currentAsset = UserAssetType.Money;

	void Start () {
		buttons.Add(UserAssetType.Money, BuyButtons[0]);
		buttons.Add(UserAssetType.Ring, BuyButtons[1]);
		buttons.Add(UserAssetType.Mobile, BuyButtons[2]);

		shopData = GameResources.Instance.GetGameData().UserAssetsShopData;

		ToggleUserAsset(currentAsset);
	}
	
	void UpdateOffers() {
		UnityUtill.DestroyByTag(Offers.transform, BuyButtonTag);
		GameData gameData = GameResources.Instance.GetGameData();

		foreach(int count in shopData.Money) {
			GameObject button = GetButtonPrefab();
			button.transform.tag = BuyButtonTag;

			UnityUtill.FindByName(button.transform, "Purchase Count").GetComponent<Text>().text = count.ToString();
			Transform priceTr = UnityUtill.FindByName(button.transform, "Price Text");
			if(priceTr != null) {
				priceTr.GetComponent<Text>().text = (count * gameData.GetPriceValue(currentAsset)).ToString();
			}
			Button btn = UnityUtill.FindByName(button.transform, "BuyButton").GetComponent<Button>();
			btn.onClick.AddListener(delegate{ OnClickBuy(count); });

			button.transform.SetParent(Offers.transform);
			button.transform.localScale = new Vector3(1, 1 ,1);
		}
	}

	void ToggleUserAsset(UserAssetType type) {
		Toggle tg = GameObject.Find(type.ToString()).GetComponent<Toggle>();
		tg.isOn = true;
	}

	GameObject GetButtonPrefab() {
		GameObject button = buttons[currentAsset];

		return Instantiate(button);
	}

	void OnClickBuy(int count) {
		Debug.Log(count);
	}

	public void OnChangeCurrentUserAsset(bool selected) {
		if(!selected) {
			return;
		}

		string name = UserAssetTypeToggleGroup.ActiveToggles().FirstOrDefault().name;
		currentAsset = EnumUtill.Parse<UserAssetType>(name);

		UpdateOffers();
	}
}
