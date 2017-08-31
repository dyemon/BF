using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Animation;

public class QuestScene : WindowScene {
	public const string SceneName = "Quest";

	public GameObjectResources GOResources;
	public VerticalLayoutGroup QuestList;
	public GameObject QuestButton;
	public UserAssetsPanel AssetsPanel;
	public GameObject AwardItem;

	public Sprite AllowBg;
	public Sprite AllowIconBg;

	private string questItemTag = "QuestItem";

	private bool save;

	void OnEnable() {
		save = false;
	}

	void OnDisable() {
		if(save) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	// Use this for initialization
	void Start () {
		UserData uData = GameResources.Instance.GetUserData();
		uData.UpDateQuests(QuestType.Game);

		UpdateQuestsInfo();
	}
	
	void UpdateQuestsInfo() {
		UserData uData = GameResources.Instance.GetUserData();
		UnityUtill.DestroyByTag(QuestList.transform, questItemTag);
		QuestData qData = GameResources.Instance.GetQuestData();

		foreach(QuestProgressData questProg in uData.GetActiveQuests(QuestType.Game, false)) {
			GameObject questGO = Instantiate(QuestButton, QuestList.transform);
			questGO.transform.tag = questItemTag;

		//	questProg.IsComplete = true;
		//	GameResources.Instance.SaveUserData(uData, false);
			GameObject allow = questGO.transform.Find("Allow").gameObject;
			GameObject notAllow = questGO.transform.Find("NotAllow").gameObject;
			allow.SetActive(questProg.IsComplete);
			notAllow.SetActive(!questProg.IsComplete);
			if(questProg.IsComplete) {
				questGO.transform.GetComponent<Image>().sprite = AllowBg;
				questGO.transform.Find("IconBg").GetComponent<Image>().sprite = AllowIconBg;
			}

			QuestItem questItem = qData.GetById(questProg.QuestId);
			GameObject active = (questProg.IsComplete) ? allow : notAllow;
			Image priceType = UnityUtill.FindByName(active.transform, "PriceType").GetComponent<Image>();
			priceType.sprite = questItem.ExperienceAward == 0 ? GOResources.GetUserAssetIcone(questItem.Award.Type) : GOResources.GetUserExperienceIcone();
			Text priceAmount = UnityUtill.FindByName(active.transform, "PriceAmount").GetComponent<Text>();
			priceAmount.text = questItem.ExperienceAward == 0 ? questItem.Award.Value.ToString() : questItem.ExperienceAward.ToString();
			priceAmount.color = questItem.ExperienceAward == 0 ? questItem.Award.Type.ToColor() : UserAssetTypeExtension.ExperienceColor;
				
			if(questProg.IsComplete) {
				allow.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => OnTakeQuest(questGO, questItem));
			}

			Text desc = questGO.transform.Find("Description").GetComponent<Text>();
			desc.text = questItem.Description;

			if(questItem.ShowProgressInfo) {
				Text progress = UnityUtill.FindByName(questGO.transform, "QuestProgress").GetComponent<Text>();
				progress.text = questProg.Progress + "/" + questItem.ActionCount;
			}

			Image icon = UnityUtill.FindByName(questGO.transform, "QuestType").GetComponent<Image>();
			icon.sprite = GOResources.GetQuestIcon(questItem);
		}
			
	}

	public void OnTakeQuest(GameObject button, QuestItem quest) {
		AssetsPanel.DisableUpdate(true);
		if(quest.ExperienceAward == 0) {
			GameResources.Instance.ChangeUserAsset(quest.Award.Type, quest.Award.Value);
		} else {
			GameResources.Instance.IncreaseExperience(quest.ExperienceAward);
		}
		GameResources.Instance.TakeQuestAward(quest.Id);
		AssetsPanel.DisableUpdate(false);

		GameObject assetImg = UnityUtill.FindByName(button.transform, "PriceType").gameObject;

		GameObject animImg = Instantiate(AwardItem, transform);
		animImg.transform.position = assetImg.transform.position;
		animImg.AddComponent<AnimatedObject>();
		Vector3 end = (quest.ExperienceAward == 0) ? AssetsPanel.GetUserAssetsIcon(quest.Award.Type).transform.position :
			AssetsPanel.GetExperienceIcon().transform.position;;
		Vector3 start = assetImg.transform.position;
		Sprite icon = (quest.ExperienceAward == 0) ? GOResources.GetUserAssetIcone(quest.Award.Type) :
			GOResources.GetUserExperienceIcone();
		int amount = (quest.ExperienceAward == 0) ? quest.Award.Value : quest.ExperienceAward; 
		Vector3? endSize = (quest.ExperienceAward != 0)? new Vector3(0.7f, 0.7f, 1) : (Vector3?)null; 

		Animations.CreateAwardAnimation(animImg, start, end, icon, amount, endSize); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteTakeQuest(animImg, button);} ).Run();



		save = true;
	}

	void CompleteTakeQuest(GameObject animGO, GameObject button) {
		Destroy(animGO);
		AssetsPanel.UpdateUserAssets();
		AssetsPanel.UpdateExperience();

		button.AddComponent<AnimatedObject>().AddFadeUI(null, 0, 1f)
			.OnStop(() => {
				Destroy(button);
				UpdateQuestsInfo();
			}).Build().Run();
	}
}
