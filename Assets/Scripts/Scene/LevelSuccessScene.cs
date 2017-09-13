using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Animation;
using UnityEngine.Advertisements;

public class LevelSuccessScene : MonoBehaviour {
	public GameObjectResources GOResources;

	public GameObject AwardItem;
	public GameObject AwardTileItem;
	public HorizontalLayoutGroup Awards1;
	public HorizontalLayoutGroup Awards2;
	public GameObject Experience;
	public Text Description;
	public UserAssetsPanel AssetPanel;

	private int successCount = 1;

	private IList<NumberScroller> awardUpdat = new List<NumberScroller>();
	private IList<BuyButton> awardUpdatBuyButtons = new List<BuyButton>();

	public Button CloseButton;
	public Button DoubleButton;
	public Button NotDoubleButton;

	private IList<GameObject> awardItems = new List<GameObject>();

	private int sumExperience;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, successCount == 1);
	}

	void Start () {
		MusicController.Play(MusicController.Instance.Default);

		int i = 0;
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		LevelAwardData awardData = levelData.SuccessAwardData;
		UserData uData = GameResources.Instance.GetUserData();
		successCount = uData.GetSuccessCount(App.CurrentLevel); 
		Preconditions.Check(successCount > 0, "Success level count for level {0} = 0", App.CurrentLevel);

		Description.text = App.CurrentLevel + ": " + levelData.Name;
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
					awardUpdat.Add(InitNumberScroller(go, awardVal + collectVal));
					awardUpdatBuyButtons.Add(go.GetComponent<BuyButton>());
				}
				awardItems.Add(go);

				GameResources.Instance.ChangeUserAsset(uData, type, awardVal + collectVal);

			}
		}
			
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards1.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards2.GetComponent<RectTransform>());

		Experience.GetComponent<BuyButton>().Init(null, GameController.CollectLevelAward.Experience, null, UserAssetTypeExtension.ExperienceColor);
		int exp = CalcAward(awardData.Experience, successCount);
		sumExperience = exp + GameController.CollectLevelAward.Experience;

		if(sumExperience > 0) {
			awardUpdat.Add(InitNumberScroller(Experience, sumExperience));
			Experience.name = "ExperienceAwardItem";
			awardUpdatBuyButtons.Add(Experience.GetComponent<BuyButton>());
			GameResources.Instance.IncreaseExperience(sumExperience);
			awardItems.Add(Experience);
		}

		AssetPanel.DisableUpdate(false);
		Invoke("UpdateAward", 1);

		ParametersController.Instance.SetParameter(ParametersController.CAN_SHOW_DAILYBONUS, true);
		ParametersController.Instance.SetParameter(ParametersController.CAN_SHOW_FORTUNA, true);
		ParametersController.Instance.SetParameter(ParametersController.CAN_SHOW_BLATHATA, true);

		App.InitAds();
	}
	
	void UpdateAward() {
		foreach(NumberScroller ns in awardUpdat) {
			ns.Run();
		}

		if(awardUpdat.Count > 0) {
			SoundController.Play(SoundController.Instance.Cash);
		}

		Invoke("CompleteUpdateAward", awardUpdat.Count > 0? 1.2f : 0);
	}

	NumberScroller InitNumberScroller(GameObject go, int endVal) {
		NumberScroller ns = go.transform.Find("PriceAmount").GetComponent<NumberScroller>();
		ns.EndValue(endVal).Speed(1).MaxDuration(1).ResetStartValue();

		return ns;
	}

	void CompleteUpdateAward() {
		foreach(BuyButton bb in awardUpdatBuyButtons) {
			bb.UpdateLayout();
		}

		if(awardItems.Count > 0) {
			DoubleButton.gameObject.SetActive(true);
			NotDoubleButton.gameObject.SetActive(true);
		} else {
			OnCompleteAwardAnimate(null);
		}
	}

	int CalcAward(int award, int successCoun) {
		return (int)Mathf.Floor(award / Mathf.Pow(2, successCount - 1));
	}

	public void OnNotDoubleClick() {
		DoubleButton.gameObject.SetActive(false);
		NotDoubleButton.gameObject.SetActive(false);
		StartAwardAnimate();
	}

	public void OnDoubleClick() {
		if(Advertisement.IsReady()) {
			Advertisement.Show(new ShowOptions() { resultCallback = AdsCallback });
		} else {
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Сервис не доступен. Проверте соединение с сетью", () => {App.InitAds(); });
		}
	}

	void AdsCallback(ShowResult res) {
		DoubleButton.gameObject.SetActive(false);
		NotDoubleButton.gameObject.SetActive(false);

		if(res == ShowResult.Finished) {
			DoubleAward();
		} else {
			StartAwardAnimate();
		}
	}

	void StartAwardAnimate() {
		AnimationGroup ag = GetComponent<AnimationGroup>();
		float delay = 0;

		foreach(GameObject item in awardItems) {
			item.GetComponent<BuyButton>().UpdateAmountFromText();
			PriceItem pi = item.GetComponent<BuyButton>().GetPriceItem();
			Sprite icon = pi == null ? GOResources.GetUserExperienceIcone() : GOResources.GetUserAssetIcone(pi.Type);
			int amount = pi == null ? sumExperience : pi.Value;
			Vector3 start = item.transform.position;
			GameObject targetGO = pi == null ? AssetPanel.GetExperienceIcon() : AssetPanel.GetUserAssetsIcon(pi.Type);
			Vector3 end = targetGO.transform.position;
			Vector3? endSize = pi == null? new Vector3(0.7f, 0.7f, 1) : (Vector3?)null; 

			GameObject animAward = Instantiate(AwardTileItem, transform);
			Animations.CreateAwardAnimation(animAward, start, end, icon, amount, endSize);
			animAward.GetComponent<AnimatedObject>().OnStop(() => {OnCompleteAward(animAward);} );

			ag.Add(animAward.GetComponent<AnimatedObject>());

			if(pi != null) {
				SoundController.Play(SoundController.Instance.Coins, SoundController.COINS_VOLUME, delay);
				delay += 0.2f;
			} else {
				SoundController.Play(SoundController.Instance.Experience, SoundController.EXPERIENCE_VOLUME, delay);
			}
		}

		if(ag.AnimationExist()) {
			ag.Run<GameObject>(OnCompleteAwardAnimate, (GameObject)null);
		} else {
			OnCompleteAwardAnimate(null);
		}
	}

	void OnCompleteAward(GameObject goAward) {
		if(goAward != null) {
			Destroy(goAward);
		}
	}

	void OnCompleteAwardAnimate(GameObject goAward) {
		AssetPanel.UpdateUserAssets();
		AssetPanel.UpdateExperience();

		CloseButton.gameObject.SetActive(true);

	}

	void DoubleAward() {
		AssetPanel.DisableUpdate(true);

		foreach(GameObject item in awardItems) {
			item.GetComponent<BuyButton>().UpdateAmountFromText();
			PriceItem pi = item.GetComponent<BuyButton>().GetPriceItem();
			int amount = pi == null ? sumExperience : pi.Value;
			InitNumberScroller(item, amount*2).Run();
			if(pi == null) {
				GameResources.Instance.IncreaseExperience(amount);
			} else {
				GameResources.Instance.ChangeUserAsset(pi.Type, pi.Value);
			}
		}

		if(awardItems.Count > 0) {
			SoundController.Play(SoundController.Instance.Cash);
		}

		sumExperience *= 2;
		AssetPanel.DisableUpdate(false);

		Invoke("CompleteDouble", 1.2f);

	}

	void CompleteDouble() {
		foreach(BuyButton bb in awardUpdatBuyButtons) {
			bb.UpdateLayout();
		}

		StartAwardAnimate();
	}
}
