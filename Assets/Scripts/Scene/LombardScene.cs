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
	public UserAssetsPanel userDataPanel;

	public GameObject[] BuyButtons;

	private IDictionary<UserAssetType, GameObject> buttons = new Dictionary<UserAssetType, GameObject>();
	private IDictionary<string, UserAssetType> toggleNames = new Dictionary<string, UserAssetType>();

	private UserAssetsShopData shopData;
	private UserAssetType currentAsset = UserAssetType.Money;

	private bool isBuy = false;

	void OnEnable() {
		isBuy = false;
	}

	void OnDisable() {
		if(isBuy) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	void Start () {
		userDataPanel.SetCurrentScene(SceneName, this);

		buttons.Add(UserAssetType.Money, BuyButtons[0]);
		buttons.Add(UserAssetType.Ring, BuyButtons[1]);
		buttons.Add(UserAssetType.Mobile, BuyButtons[2]);

		shopData = GameResources.Instance.GetGameData().UserAssetsShopData;
		if(SceneControllerHelper.instance != null) {
			System.Object param = SceneControllerHelper.instance.GetParameter(SceneName);
			if(param != null) {
				currentAsset = (UserAssetType)param;
			}
		}
		if(currentAsset == null) {
			currentAsset = UserAssetType.Money;
		}

		ToggleUserAsset(currentAsset);
	}
	
	void UpdateOffers() {
		UnityUtill.DestroyByTag(Offers.transform, BuyButtonTag);
		GameData gameData = GameResources.Instance.GetGameData();
		int[] offers = shopData.Money;
		if(currentAsset == UserAssetType.Mobile) {
			offers = shopData.Mobile;
		} else if(currentAsset == UserAssetType.Ring) {
			offers = shopData.Ring;
		}

		foreach(int count in offers) {
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
			button.name = "BuyButton" + count; 
		}
	}

	void ToggleUserAsset(UserAssetType type) {
		GameObject obj = GameObject.Find(type.ToString());
		Toggle tg =	obj.GetComponent<Toggle>();
		tg.isOn = true;
	}

	GameObject GetButtonPrefab() {
		GameObject button = buttons[currentAsset];

		return Instantiate(button);
	}

	public void OnChangeCurrentUserAsset(bool selected) {
		if(!selected) {
			return;
		}

		string name = UserAssetTypeToggleGroup.ActiveToggles().FirstOrDefault().name;
		currentAsset = EnumUtill.Parse<UserAssetType>(name);

		UpdateOffers();
	}

	void OnClickBuy(int count) {
	//	if(currentAsset == UserAssetType.Money) {
	//		return;
	//	}

		BuyUserAsset(count);
	}

	void BuyUserAsset(int count) {
		userDataPanel.DisableUpdate(true);
		if(!GameResources.Instance.Buy(currentAsset, count)) {
			DisplayMessageController.DisplayNotEnoughMessage(UserAssetType.Money);
			ToggleUserAsset(UserAssetType.Money);
			userDataPanel.DisableUpdate(false);
			return;
		}

		isBuy = true;


		GameObject assetImg = UnityUtill.FindByName(Offers.transform, "BuyButton" + count)
			.Find("Icon/Image").gameObject;
		
		GameObject animImg = Instantiate(assetImg, assetImg.transform);
		animImg.AddComponent<AnimatedObject>();
		Vector3 end = userDataPanel.GetUserAssetsIcon(currentAsset).transform.position;
		Vector3 start = assetImg.transform.position;

		Animations.CreateAwardAnimation(animImg, start, end, null, null); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => CompleteBuyUserAsset(currentAsset, count, animImg) ).Run();
	}

	void CompleteBuyUserAsset(UserAssetType type, int count, GameObject animImg) {
		Destroy(animImg);
		userDataPanel.DisableUpdate(false);
		userDataPanel.UpdateUserAssets();
	}
}
