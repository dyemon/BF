using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EnergyScene : WindowScene {
	public static string SceneName = "Energy";

	public Sprite[] EnergyIcons;
	public RectTransform Offers;
	public string BuyButtonTag;
	public UserAssetsPanel userDataPanel;

	public GameObject BuyButtons;


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

		shopData = GameResources.Instance.GetGameData().UserAssetsShopData;
		if(SceneControllerHelper.instance != null) {
			currentAsset = (UserAssetType)SceneControllerHelper.instance.GetParameter(SceneName);
		}
		if(currentAsset == null) {
			currentAsset = UserAssetType.Money;
		}

		UpdateOffers();
	}
	
	void UpdateOffers() {
		GameData gameData = GameResources.Instance.GetGameData();

		foreach(int count in shopData.Energy) {
			GameObject button = GetButtonPrefab();

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


	GameObject GetButtonPrefab() {
		return Instantiate(BuyButtons);
	}



	void OnClickBuy(int count) {
	//	if(currentAsset == UserAssetType.Money) {
	//		return;
	//	}

		BuyUserAsset(count);
	}

	void BuyUserAsset(int count) {
		if(!GameResources.Instance.Buy(currentAsset, count)) {
			DisplayMessageController.DisplayNotEnoughMessage(UserAssetType.Money);
			return;
		}

		isBuy = true;

		GameObject assetImg = UnityUtill.FindByName(Offers.transform, "BuyButton" + count)
			.Find("Icon/Image").gameObject;
		
		GameObject animImg = Instantiate(assetImg, assetImg.transform);
		AnimatedObject ao = animImg.AddComponent<AnimatedObject>();

		float speed = App.GetTileItemSpeed(TileItemMoveType.BUY_USERASSET);
		Vector3 end = userDataPanel.GetUserAssetsIcon(currentAsset).transform.position;
		float time = AMove.CalcTime(animImg.transform.position, end, speed);

		ao.AddMove(null, end, speed).AddResize(null, new Vector3(0.5f, 0.5f, 1f), time)
		.OnStop(() => CompleteBuyUserAsset(currentAsset, count, animImg) )
		.Build().Run();

	}

	void CompleteBuyUserAsset(UserAssetType type, int count, GameObject animImg) {
		Destroy(animImg);
		userDataPanel.UpdateUserAssets();
	}
}
