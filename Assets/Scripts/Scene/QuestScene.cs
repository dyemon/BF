using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestScene : WindowScene {
	public const string SceneName = "Quest";

	public GameObjectResources GOResources;
	public VerticalLayoutGroup QuestList;
	public GameObject QuestButton;

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

			GameObject allow = questGO.transform.Find("Allow").gameObject;
			GameObject notAllow = questGO.transform.Find("NotAllow").gameObject;
			allow.SetActive(questProg.IsComplete);
			notAllow.SetActive(!questProg.IsComplete);

			QuestItem questItem = qData.GetById(questProg.QuestId);
			GameObject active = (questProg.IsComplete) ? allow : notAllow;
			Image priceType = UnityUtill.FindByName(active.transform, "PriceType").GetComponent<Image>();
			priceType.sprite = questItem.ExperienceAward == 0 ? GOResources.GetUserAssetIcone(questItem.Award.Type) : GOResources.GetUserExperienceIcone();
			Text priceAmount = UnityUtill.FindByName(active.transform, "PriceAmount").GetComponent<Text>();
			priceAmount.text = questItem.ExperienceAward == 0 ? questItem.Award.Value.ToString() : questItem.ExperienceAward.ToString();
			priceAmount.color = questItem.ExperienceAward == 0 ? questItem.Award.Type.ToColor() : UserAssetTypeExtension.ExperienceColor;
				
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
}
