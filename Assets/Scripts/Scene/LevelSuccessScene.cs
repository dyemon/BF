using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSuccessScene : MonoBehaviour {
	public GameObject AwardItem;
	public HorizontalLayoutGroup Awards1;
	public HorizontalLayoutGroup Awards2;
	public GameObject Experience;
	public Text Description;
	public UserAssetsPanel AssetPanel;

	private int successCount = 1;

	private IList<NumberScroller> awardUpdat = new List<NumberScroller>();
	private IList<BuyButton> awardUpdatBuyButtons = new List<BuyButton>();

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, successCount == 1);
	}

	void Start () {
		int i = 0;
		LevelData levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		LevelAwardData awardData = levelData.SuccessAwardData;
		UserData uData = GameResources.Instance.GetUserData();
		successCount = uData.GetSuccessCount(App.GetCurrentLevel()); 
		Preconditions.Check(successCount > 0, "Success level count for level {0} = 0", App.GetCurrentLevel());

		Description.text = App.GetCurrentLevel() + ": " + levelData.Name;
		AssetPanel.DisableUpdate(true);

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			int collectVal = GameController.CollectLevelAward.GetAsset(type);
			int awardVal = awardData.GetAsset(type);
			awardVal = CalcAward(awardVal, successCount); 

			if(collectVal > 0 || awardVal > 0) {
				HorizontalLayoutGroup layout = (i++ > 1) ? Awards2 : Awards1;
				GameObject go = Instantiate(AwardItem, layout.transform);
				go.name = type.ToString() + "AwardItem";
				go.GetComponent<BuyButton>().Init(type, collectVal, null);
	
				if(awardVal > 0) {
					GameResources.Instance.ChangeUserAsset(uData, type, awardVal);
					awardUpdat.Add(InitNumberScroller(go, awardVal + collectVal));
					awardUpdatBuyButtons.Add(go.GetComponent<BuyButton>());
				}

			}
		}
			
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards1.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards2.GetComponent<RectTransform>());

		Experience.GetComponent<BuyButton>().Init(null, GameController.CollectLevelAward.Experience, null, UserAssetTypeExtension.ExperienceColor);
		int exp = CalcAward(awardData.Experience, successCount);

		if(exp > 0) {
			awardUpdat.Add(InitNumberScroller(Experience, exp + GameController.CollectLevelAward.Experience));
			Experience.name = "ExperienceAwardItem";
			awardUpdatBuyButtons.Add(Experience.GetComponent<BuyButton>());
			GameResources.Instance.IncreaseExperience(exp);
		}

		AssetPanel.DisableUpdate(false);
		Invoke("UpdateAward", 1);
	}
	
	void UpdateAward() {
		foreach(NumberScroller ns in awardUpdat) {
			ns.Run();
		}

		Invoke("CompleteUpdateAward", 1.2f);
	}

	NumberScroller InitNumberScroller(GameObject go, int endVal) {
		NumberScroller ns = go.transform.Find("PriceAmount").GetComponent<NumberScroller>();
		ns.EndValue(endVal).Speed(1).MaxDuration(1);

		return ns;
	}

	void CompleteUpdateAward() {
		foreach(BuyButton bb in awardUpdatBuyButtons) {
			bb.UpdateLayout();
		}

		AssetPanel.UpdateExperience();
		AssetPanel.UpdateUserAssets();
	}

	int CalcAward(int award, int successCoun) {
		return (int)Mathf.Floor(award / Mathf.Pow(2, successCount - 1));
	}
}
