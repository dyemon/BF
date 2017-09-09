using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Animation;

public class KachalkaScene : WindowScene {
	public const string SceneName = "Kachalka";

	public GameObjectResources GOResource;
	public GameObject AwaliableItem;
	public GameObject UnawaliableItem;
	public GameObject ItemsList;

	public GameObject OwnUserDamage;
	public GameObject OwnUserHealth;

	private KachalkaData[] kachalkaDataItems;
	private bool save;

	void OnEnable() {
		save = false;
	}

	void OnDisable() {
		if(save) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	protected override void Start() {
		base.Start();
		kachalkaDataItems = GameResources.Instance.GetGameData().KachalkaDataItems;

		UpdateKachalkaItems();
		UpdateOwnUserData();
	}
	
	void UpdateKachalkaItems() {
		UnityUtill.DestroyByTag(ItemsList.transform, AwaliableItem.transform.tag);
		UserData uData = GameResources.Instance.GetUserData();

		foreach(KachalkaData kData in kachalkaDataItems) {
			int itemIndex = 0;
			int stepIndex = - 1;
			UserKachalkaItem item = uData.GetKachalkaItem(kData.Type);
			if(item != null) {
				itemIndex = item.ItemIndex;
				stepIndex = item.StepIndex;
			}

			KachalkaItem kItem = kData.Items[itemIndex];
			if(stepIndex >= kItem.Steps.Length - 1) {
				itemIndex++;
				stepIndex = -1;

			}
			if(itemIndex >= kData.Items.Length - 1) {
				continue;
			}

			kItem = kData.Items[itemIndex];
			PriceItem stepItem = kItem.Steps[stepIndex + 1];

			bool avaliable = false;
			if(stepIndex >= 0) {
				avaliable = true;
			} else {
				avaliable = kItem.MinExperience <= uData.Experience;
			}

			GameObject newItemGO = Instantiate(avaliable ? AwaliableItem : UnawaliableItem, ItemsList.transform);

			newItemGO.transform.Find("IconBg/Effect/EffectType").GetComponent<Image>()
				.sprite = kItem.Damage > 0 ? GOResource.GetDamageIcone() : GOResource.GetHealthIcone();
			newItemGO.transform.Find("IconBg/Effect/EffectAmount").GetComponent<Text>()
				.text = "+" + ((kItem.Damage > 0) ? kItem.Damage : kItem.Health);
			newItemGO.transform.Find("Description").GetComponent<Text>()
				.text = kData.Name;

			if(avaliable) {
				Button b = newItemGO.transform.Find("Button").GetComponent<Button>();
				b.GetComponent<BuyButton>().Init(stepItem.Type, stepItem.Value, null);
				b.onClick.AddListener(() => {
					OnClickKachat(kItem, kData.Type, newItemGO);
				});

				Slider slider = newItemGO.transform.Find("Slider").GetComponent<Slider>();
				slider.maxValue = kItem.Steps.Length;
				slider.value = stepIndex + 1;

				slider.transform.Find("Text").GetComponent<Text>()
					.text = (stepIndex + 1) + "/" + kItem.Steps.Length;
			} else {
				newItemGO.transform.Find("MinExperience").GetComponent<Text>()
					.text = kItem.MinExperience.ToString();
				newItemGO.transform.Find("MinExperience").GetComponent<Text>()
					.color = UserAssetTypeExtension.ExperienceColor;
			}
		}
	}


	void OnClickKachat(KachalkaItem kItem, KachalkaType type, GameObject itemGO) {
		UserData uData = GameResources.Instance.GetUserData();
		UserKachalkaItem ukItem = uData.GetKachalkaItem(type);
		int stepIndex = -1;
		if(ukItem != null) {
			stepIndex = ukItem.StepIndex;
		}

		PriceItem stepItem = kItem.Steps[stepIndex + 1];
		if(!GameResources.Instance.ChangeUserAsset(stepItem.Type, -stepItem.Value)) {
			SceneController.Instance.ShowUserAssetsScene(stepItem.Type, true);
			return;
		}

		if(!GameResources.Instance.IncreaseKachalkaIndex(type)) {
			UpdateKachalkaItems();
		} else {
			Slider slider = itemGO.transform.Find("Slider").GetComponent<Slider>();
			slider.value = slider.maxValue;

			GameObject source = itemGO.transform.Find("IconBg/Effect/EffectType").gameObject;
			GameObject aItem = Instantiate(source, transform);
			Vector3 start = source.transform.position;
			Vector3 end = kItem.Damage > 0 ? OwnUserDamage.transform.position : OwnUserHealth.transform.position;
			AnimatedObject ao = aItem.AddComponent<AnimatedObject>();

			Animations.CreateAwardAnimation(aItem, start, end, null, null, new Vector3(1f, 1f, 1));
			ao.OnStop(() => {
				OnCompleteAnimate(aItem);
			}).Run();
		}

		SoundController.Play(SoundController.Instance.Kassa, SoundController.KASSA_VOLUME);

		save = true;
	}

	void UpdateOwnUserData() {
		UserData uData = GameResources.Instance.GetUserData();
		OwnUserDamage.transform.Find("Text").GetComponent<Text>().text = uData.OwnDamage.ToString();
		OwnUserHealth.transform.Find("Text").GetComponent<Text>().text = uData.OwnHealth.ToString();
	}

	void OnCompleteAnimate(GameObject aItemGO) {
		Destroy(aItemGO);
		UpdateKachalkaItems();
		UpdateOwnUserData();
		SoundController.Play(SoundController.Instance.Stat, SoundController.STAT_VOLUME);

	}
}
