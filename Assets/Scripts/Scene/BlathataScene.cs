﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Common.Animation;

public class BlathataScene : WindowScene {
	public const string SceneName = "Blathata";

	public GameObjectResources GOResources;
	public GameObject[] awardBoxes;
	public UserAssetsPanel AssetsPanel;
	public GameObject AwardItem;
	public Text PriceText;
	public Text StarCountText;
	public GameObject BuyStarStartPos;
	public GameObject BuyStartGO;

	private int starPrice;
	const int STAR_BUY_COUNT = 3;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
	}

	protected override void Start() {
		base.Start();

		foreach(GameObject item in awardBoxes) {
			item.GetComponent<Button>().onClick
				.AddListener(() => {OnSelectBox(item);});
		}

		GameData gData = GameResources.Instance.GetGameData();
		starPrice = gData.GetPriceValue(UserAssetType.Star) * STAR_BUY_COUNT;
		PriceText.text = starPrice.ToString();
		UpdateStarCount();

		ParametersController.Instance.SetParameter(ParametersController.BLATHATA_IS_SHOWN, true);
	}
	
	void OnSelectBox(GameObject box) {
		if(box.GetComponent<CanvasGroup>().alpha != 1) {
			return;
		}

		AwardItem award = GameResources.Instance.GetGameData().BlathataData.GetAward();

		AssetsPanel.DisableUpdate(true);
		if(!GameResources.Instance.ChangeUserAsset(UserAssetType.Star, -1)) {
			BuyStartGO.GetComponent<AnimatedObject>()
				.AddResize(null, new Vector3(1.3f, 1.3f, 1), 0.3f)
				.AddResize(null, new Vector3(1f, 1f, 1), 0.3f)
				.Build().Run();
			
			AssetsPanel.DisableUpdate(false);
			SoundController.Play(SoundController.Instance.FaltAction);

			return;
		}
		GameResources.Instance.ChangeUserAsset(award.Type, award.Value);
		AssetsPanel.DisableUpdate(false);

	
		GameObject animImg = Instantiate(AwardItem, box.transform.position, Quaternion.identity);
		animImg.transform.SetParent(transform);
		animImg.AddComponent<AnimatedObject>();
		Vector3 end = AssetsPanel.GetUserAssetsIcon(award.Type).transform.position;
		Vector3 start = box.transform.position;
		Sprite icon = GOResources.GetUserAssetIcone(award.Type);

		Animations.CreateAwardAnimation(animImg, start, end, icon, award.Value); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteTakeBox(animImg, box);} ).Run();

		box.GetComponent<AnimatedObject>().AddFadeUI(null, 0, 1f).Build().Run();
		SoundController.Play(SoundController.Instance.Coins, SoundController.COINS_VOLUME);

	}

	void CompleteTakeBox(GameObject animGO, GameObject box) {
		Destroy(animGO);
		AssetsPanel.UpdateUserAssets();
		if(box != null) {
			StartCoroutine(FadeInBox(box));
		}
		UpdateStarCount();
	}

	IEnumerator FadeInBox(GameObject box) {
		yield return new WaitForSeconds(5);
		box.GetComponent<AnimatedObject>().AddFadeUI(null, 1, 1f).Build().Run();
	}

	void UpdateStarCount() {
		UserData uData = GameResources.Instance.GetUserData();
		StarCountText.text = uData.GetAsset(UserAssetType.Star).Value.ToString();
	}

	public void OnClickBuyStar() {
		AssetsPanel.DisableUpdate(true);
		if(!GameResources.Instance.ChangeUserAsset(UserAssetType.Money, -starPrice)) {
			SceneController.Instance.ShowUserAssetsScene(UserAssetType.Money, true);
			AssetsPanel.DisableUpdate(false);
			return;
		}
		GameResources.Instance.ChangeUserAsset(UserAssetType.Star, STAR_BUY_COUNT);
		AssetsPanel.DisableUpdate(false);

		GameObject animImg = Instantiate(AwardItem, transform);
		animImg.AddComponent<AnimatedObject>();
		Vector3 start = BuyStarStartPos.transform.position;
		Vector3 end = AssetsPanel.GetUserAssetsIcon(UserAssetType.Star).transform.position;
		Sprite icon = GOResources.GetUserAssetIcone(UserAssetType.Star);

		Animations.CreateAwardAnimation(animImg, start, end, icon, STAR_BUY_COUNT); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteTakeBox(animImg, null);} ).Run();
		SoundController.Play(SoundController.Instance.Kassa, SoundController.KASSA_VOLUME);

	}
}
