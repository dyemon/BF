using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Common.Animation;

public class EnergyScene : WindowScene {
	public GameObjectResources GOResources;

	public static string SceneName = "Energy";

	public Sprite[] EnergyIcons;
	public Sprite[] InfinityEnergyIcons;
	private string[] energyNameText = {"Пакетик семечек", "Стакан семечек", "Большой стакан семечек"};
	private string[] infinityEnergyNameText = {"Бесконечная энергия"};

	public RectTransform Offers;
	public string BuyButtonTag;
	public UserAssetsPanel userDataPanel;

	public GameObject BuyButtons;
	public GameObject BuyInfinityButtons;

	private UserAssetsShopData shopData;
	private EnergyData energyData;

	private bool isBuy = false;

	void OnEnable() {
		isBuy = false;
	}

	void OnDisable() {
		if(isBuy) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	protected override void Start() {
		base.Start();
		
		userDataPanel.SetCurrentScene(SceneName, this);
		shopData = GameResources.Instance.GetGameData().UserAssetsShopData;
		energyData = GameResources.Instance.GetGameData().EnergyData;
		UpdateOffers();

	}
	
	void UpdateOffers() {
		GameData gameData = GameResources.Instance.GetGameData();
		int i = 0;

		foreach(int count in shopData.Energy) {
			if(i >= EnergyIcons.Length || i >= energyNameText.Length) {
				break;
			}

			GameObject button = GetButtonPrefab(false);

			UnityUtill.FindByName(button.transform, "PriceAmount").GetComponent<Text>().text = count.ToString();
			Transform priceTr = UnityUtill.FindByName(button.transform, "Price Text");
			if(priceTr != null) {
				priceTr.GetComponent<Text>().text = (count * gameData.GetPriceValue(UserAssetType.Energy)).ToString();
			}
			Button btn = UnityUtill.FindByName(button.transform, "BuyButton").GetComponent<Button>();
			btn.onClick.AddListener(delegate{ OnClickBuy(count, false); });

			Text name = UnityUtill.FindByName(button.transform, "Name").GetComponent<Text>();
			name.text = energyNameText[i];

			Image icon = UnityUtill.FindByName(button.transform, "Icon").Find("Image").GetComponent<Image>();
			icon.sprite = EnergyIcons[i];

			button.transform.SetParent(Offers.transform);
			button.transform.localScale = new Vector3(1, 1 ,1);
			button.name = "BuyButton" + count; 

			i++;
		}

		i = 0;
		foreach(int count in shopData.InfinityEnergy) {
			if(i >= InfinityEnergyIcons.Length || i >= energyNameText.Length) {
				break;
			}

			GameObject button = GetButtonPrefab(true);

			UnityUtill.FindByName(button.transform, "Purchase Count").GetComponent<Text>().text = string.Format("{0} часа", count);
			Transform priceTr = UnityUtill.FindByName(button.transform, "Price Text");
			if(priceTr != null) {
				priceTr.GetComponent<Text>().text = (count * energyData.InfinityPrice).ToString();
			}
			Button btn = UnityUtill.FindByName(button.transform, "BuyButton").GetComponent<Button>();
			btn.onClick.AddListener(delegate{ OnClickBuy(count, true); });

			Text name = UnityUtill.FindByName(button.transform, "Name").GetComponent<Text>();
			name.text = infinityEnergyNameText[i];

			Image icon = UnityUtill.FindByName(button.transform, "Icon").Find("Image").GetComponent<Image>();
			icon.sprite = InfinityEnergyIcons[i];

			button.transform.SetParent(Offers.transform);
			button.transform.localScale = new Vector3(1, 1 ,1);
			button.name = "BuyButton" + count; 

			i++;
		}
	}


	GameObject GetButtonPrefab(bool isInfinity) {
		return (isInfinity)? Instantiate(BuyInfinityButtons) : Instantiate(BuyButtons);
	}



	void OnClickBuy(int count, bool isInfinity) {
		bool canBuy = false;

		userDataPanel.DisableUpdate(true);
		if(!isInfinity) {
			canBuy = GameResources.Instance.Buy(UserAssetType.Energy, count);
		} else {
			canBuy = GameResources.Instance.BuyInfinityEnergy(count);
		}
		if(!canBuy) {
			userDataPanel.DisableUpdate(false);
			if(SceneController.Instance.IsSceneEist(LombardScene.SceneName)) {
				Close();
			}
			SceneControllerHelper.instance.ShowUserAssetsScene(UserAssetType.Money, true);
			return;
		}

		isBuy = true;
		SoundController.Play(SoundController.Instance.Kassa, 3);
		GameObject assetImg = UnityUtill.FindByName(Offers.transform, "BuyButton" + count)
			.Find("Icon/Image").gameObject;

	//	GameObject icon = (isInfinity) ? assetImg : (GameObject)GameObjectResources.GetUserAssetIcone(UserAssetType.Energy);
		GameObject animImg = Instantiate(assetImg, assetImg.transform.position, Quaternion.identity);
		animImg.transform.SetParent(transform);
		animImg.transform.localScale = Vector3.one;
		if(!isInfinity) {
			animImg.GetComponent<Image>().sprite = GOResources.GetUserAssetIcone(UserAssetType.Energy);
		}

		AnimatedObject ao = animImg.AddComponent<AnimatedObject>();
	
		Vector3 end;
		if(!isInfinity) {
			end = userDataPanel.GetUserAssetsIcon(UserAssetType.Energy).transform.position;
		} else {
			end = userDataPanel.GetInfinityEnergyObject().transform.position;
		}

		Animations.CreateAwardAnimation(animImg, animImg.transform.position , end, null, null, new Vector3(0.5f, 0.5f,1)); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => CompleteBuyUserAsset(UserAssetType.Energy, count, animImg, isInfinity) ).Run();
	}



	void CompleteBuyUserAsset(UserAssetType type, int count, GameObject animImg, bool isInfinity) {
		Destroy(animImg);
		userDataPanel.DisableUpdate(false);
		if(isInfinity) {
			userDataPanel.OnUpdateInfinityEnergy(count);
			GameTimers.Instance.StartInfinityEnergyTimer();
		} else {
			userDataPanel.UpdateUserAssets();
		}
	}


}
