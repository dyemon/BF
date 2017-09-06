using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Animation;

public class GoodsScene : WindowScene {
	public const string SceneName = "Goods";

	public GameObjectResources GOResources;

	public GameObject AwaliableItem;
	public GameObject UnawaliableItem;
	public GameObject GoodsItemsList;

	public GameObject UserHealth;
	public GameObject UserDamage;

	public UserAssetsPanel AssetPanel;
	private bool save;

	public ScrollRect GoodsScrollRect;

	void OnEnable() {
		save = false;
	}

	void OnDisable() {
		if(save) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	// Use this for initialization
	protected override void Start() {
		base.Start();
		UpdateGoodsItems();
		UpdateUserEquipment();
	}
	
	void UpdateGoodsItems() {
		UnityUtill.DestroyByTag(GoodsItemsList.transform, AwaliableItem.transform.tag);

		UserData uData = GameResources.Instance.GetUserData();
		GameData gameData = GameResources.Instance.GetGameData();

		GoodsData userHealth = gameData.GetGoodsData(uData.HealthEquipmentType);
		GoodsData userDamage = gameData.GetGoodsData(uData.DamageEquipmentType);

		GoodsScrollRect.horizontalNormalizedPosition = 0;

		IList<GoodsData> healthList = new List<GoodsData>();
		IList<GoodsData> damageList = new List<GoodsData>();
		bool isHealth;

		foreach(GoodsData goodsData in gameData.GoodsData) {
			isHealth = goodsData.Health > 0;

			if(isHealth && userHealth != null && userHealth.Health >= goodsData.Health) {
				continue;
			}
			if(!isHealth && userDamage != null && userDamage.Damage >= goodsData.Damage) {
				continue;
			}

			if(isHealth) {
				healthList.Add(goodsData);
			} else {
				damageList.Add(goodsData);
			}
		}

		int healthIndex = 0;
		int damageIndex = 0;
		isHealth = true;

		while(healthIndex < healthList.Count || damageIndex < damageList.Count) {
			GoodsData goodsData = null;
			isHealth = !isHealth;
			if(isHealth) {
				if(healthList.Count <= healthIndex) {
					goodsData = damageList[damageIndex++];
					isHealth = false;
				} else {
					goodsData = healthList[healthIndex++];
				}
			} else {
				if(damageList.Count <= damageIndex) {
					goodsData = healthList[healthIndex++];
					isHealth = true;
				} else {
					goodsData = damageList[damageIndex++];
				}
			}

			bool isAvaliable = goodsData.MinExperience <= uData.Experience;

			GameObject newGO = Instantiate(isAvaliable ? AwaliableItem : UnawaliableItem, GoodsItemsList.transform);
			newGO.name = goodsData.Type.ToString();

			newGO.transform.Find("GoodsIcon").GetComponent<Image>()
				.sprite = GOResources.GetGoodsIcon(goodsData.Type);
			newGO.transform.Find("EffectType").GetComponent<Image>()
				.sprite = isHealth ? GOResources.GetHealthIcone() : GOResources.GetDamageIcone();
			newGO.transform.Find("EffectAmount").GetComponent<Text>()
				.text = isHealth ? goodsData.Health.ToString() : goodsData.Damage.ToString();

			if(isAvaliable) {
				Button b = newGO.transform.Find("BuyButton").GetComponent<Button>();
				b.GetComponent<BuyButton>().Init(goodsData.PriceType, goodsData.PriceValue, null);
				b.onClick.AddListener(() => {
					OnBuyGoods(goodsData, newGO);
				}); 
			} else {
				Text expText = newGO.transform.Find("BuyButton/PriceAmount").GetComponent<Text>();
				expText.text = goodsData.MinExperience.ToString();
				expText.color = UserAssetTypeExtension.ExperienceColor;
			}
		}
	}

	void UpdateUserEquipment() {
		UserData uData = GameResources.Instance.GetUserData();
		GameData gameData = GameResources.Instance.GetGameData();

		GoodsData userHealth = gameData.GetGoodsData(uData.HealthEquipmentType);
		GoodsData userDamage = gameData.GetGoodsData(uData.DamageEquipmentType);

		UpdateUserGoddsItem(userHealth, UserHealth);
		UpdateUserGoddsItem(userDamage, UserDamage);
	}
	/*
	void UpdateGoodsItems() {
		UserData uData = GameResources.Instance.GetUserData();
		GameData gameData = GameResources.Instance.GetGameData();

		GoodsData userHealth = gameData.GetGoodsData(uData.HealthEquipmentType);
		GoodsData userDamage = gameData.GetGoodsData(uData.DamageEquipmentType);

		foreach(GoodsData goodsData in gameData.GoodsData) {
			bool isHealth = goodsData.Health > 0;

			if((isHealth && userHealth != null && userHealth.Health >= goodsData.Health) ||
				(!isHealth && userDamage != null && userDamage.Damage >= goodsData.Damage)) {
				Transform tr = UnityUtill.FindByName(GoodsItemsList.transform, goodsData.Type.ToString());
				if(tr != null) {
					Destroy(tr.gameObject);
				}
			}
		}

		GoodsScrollRect.horizontalNormalizedPosition = 0;
	}
	*/
	void UpdateUserGoddsItem(GoodsData data, GameObject go) {
		if(data == null) {
			go.SetActive(false);
		} else {
			go.SetActive(true);
			go.GetComponent<Image>().sprite = GOResources.GetGoodsIcon(data.Type);
			go.transform.Find("Text").GetComponent<Text>().text = data.Health > 0 ? data.Health.ToString() : data.Damage.ToString();
		}
	}

	void OnBuyGoods(GoodsData data, GameObject go) {
		AssetPanel.DisableUpdate(true);
		if(!GameResources.Instance.Buy(data)) {
			AssetPanel.DisableUpdate(false);
			return;
		}
		AssetPanel.DisableUpdate(true);
		save = true;

		GameObject source = go.transform.Find("GoodsIcon").gameObject;
		GameObject aItem = Instantiate(source, transform);
		Vector3 start = source.transform.position;
		Vector3 end = data.Health > 0 ? UserHealth.transform.position : UserDamage.transform.position;
		AnimatedObject ao = aItem.AddComponent<AnimatedObject>();

		Animations.CreateAwardAnimation(aItem, start, end, null, null, new Vector3(1f, 1f, 1));
		ao.OnStop(() => {
			OnCompleteAnimate(aItem);
		}).Run();

	}

	void OnCompleteAnimate(GameObject aItemGO) {
		Destroy(aItemGO);
		AssetPanel.UpdateUserAssets();
		UpdateUserEquipment();
		UpdateGoodsItems();
	}
}
