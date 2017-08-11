using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Animation;

public class FBLoggedScene : WindowScene, IFBCallback {
	public const string SceneName = "FBLogged";

	public GameObjectResources GOResources;

	public Button TakenAwardBtn;
	public GameObject QuestDescription;
	public GameObject QuestInfo;
	public FBController FBController;
	public Text UserName;

	private bool save;

	void OnEnable() {
		save = false;
	}

	void OnDisable() {
		if(save) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {

	}

	public void OnFriendsRequest(IList<FBUser> friends) {
		UserData uData = GameResources.Instance.GetUserData();
		QuestProgressData pd = uData.GetActiveQuestOne(QuestType.SocialFB, false);
		if(pd != null) {
			QuestItem questItem = GameResources.Instance.GetQuestData().GetById(pd.QuestId);
			if(questItem.ConditionType == QuestConditionType.InviteFriends) {
				pd = GameResources.Instance.IncreasQuestAction(questItem.Id, friends.Count, false, true);
			}
		}

		UpdateQuestInfo(pd);
		ShowQuestInfo(true);
	}

	public void OnFBLogout() {
		SceneController.Instance.UnloadScene("FBLogged");

	}


	public void OnFBLoginFail(string error) {
	}
		

	void Start() {
		TakenAwardBtn.gameObject.SetActive(false);
		QuestDescription.SetActive(false);
		UserName.text = Account.Instance.FBUser.Name;

		UserData uData = GameResources.Instance.GetUserData();
		QuestProgressData pd = uData.GetActiveQuestOne(QuestType.SocialFB, false);
		if(pd != null) {
			QuestItem questItem = GameResources.Instance.GetQuestData().GetById(pd.QuestId);
			if(questItem.ConditionType == QuestConditionType.InviteFriends) {
				FBController.RequestFriendsList();
				return;
			}
		}

		UpdateQuestInfo(pd);

	}

	void UpdateQuestInfo(QuestProgressData quest) {
		if(quest == null) {
			return;
		}

		QuestItem questItem = GameResources.Instance.GetQuestData().GetById(quest.QuestId);
		Transform awardItem = TakenAwardBtn.transform.Find("AwardItem");
		Image img = awardItem.Find("Image").GetComponent<Image>();
		img.sprite = GOResources.GetUserAssetIcone(questItem.Award.Type);
		Text text = awardItem.Find("Text").GetComponent<Text>();
		text.text = questItem.Award.Value.ToString();

		int progress = quest.Progress > questItem.ActionCount? questItem.ActionCount : quest.Progress;
		string descr = questItem.Description + (questItem.ShowProgressInfo? string.Format(" ({0}/{1})", progress, questItem.ActionCount) : "");
		QuestDescription.transform.Find("Text").GetComponent<Text>().text = descr;

		TakenAwardBtn.gameObject.SetActive(true);
		QuestDescription.SetActive(true);

		if(quest.IsComplete) {
			TakenAwardBtn.GetComponent<CanvasGroup>().alpha = 1;
			TakenAwardBtn.interactable = true;
		} else {
			TakenAwardBtn.GetComponent<CanvasGroup>().alpha = 0.5f;
			TakenAwardBtn.interactable = false;
		}
	}

	public void OnClickTakeAward() {
		UserData uData = GameResources.Instance.GetUserData();
		QuestProgressData qData = uData.GetActiveQuestOne(QuestType.SocialFB, false);
		QuestItem questItem = GameResources.Instance.GetQuestData().GetById(qData.QuestId);

		GameResources.Instance.ChangeUserAsset(questItem.Award.Type, questItem.Award.Value);
		if(questItem.ExperienceAward > 0) {
			GameResources.Instance.IncreaseExperience(questItem.ExperienceAward);
		}

		StartCoroutine(AnimateAward(questItem, false));
		if(questItem.ExperienceAward > 0) {
			StartCoroutine(AnimateAward(questItem, true));
		}

		AnimatedObject ao = QuestInfo.GetComponent<AnimatedObject>();
		ao.AddFadeUI(null, 0, 1).Build().Run();

		GameResources.Instance.TakeQuestAward(questItem.Id);

		save = true;

		Invoke("UpdateQuest", 2);
	}

	IEnumerator AnimateAward(QuestItem questItem, bool isExperience) {
		if(isExperience) {
			yield return new WaitForSeconds(0.2f);
		}

		GameObject awardItem = TakenAwardBtn.transform.Find("AwardItem").gameObject;
		GameObject animItem = Instantiate(awardItem, awardItem.transform.position, Quaternion.identity);
		animItem.transform.SetParent(transform);
		animItem.transform.localScale = Vector3.one;

		if(isExperience) {
			Image img = animItem.transform.Find("Image").GetComponent<Image>();
			img.sprite = GOResources.GetUserExperienceIcone();
			Text text = animItem.transform.Find("Text").GetComponent<Text>();
			text.text = questItem.ExperienceAward.ToString();
		}

		Vector3 start = animItem.transform.position;
		float dist = Screen.height / 2f;
		Vector3 end1 = start + new Vector3(0, dist*0.5f, 0);
		Vector3 end2 = start + new Vector3(0, dist, 0);

		float time1 = App.GetMoveTime(UIMoveType.AWARD_EXPERIENCE);
		float time2 = App.GetMoveTime(UIMoveType.AWARD_EXPERIENCE);

		AnimatedObject ao = animItem.AddComponent<AnimatedObject>();
		ao.AddMoveByTime(start, end1, time1).Build()
			.AddMoveByTime(null, end2, time2).AddFadeUI(null, 0f, time2)
			.OnStop(() => {} ).Build()
			.Run();

		Destroy(animItem, 2f);
	}

	void UpdateQuest() {
		UserData uData = GameResources.Instance.GetUserData();
		QuestProgressData qData = uData.GetActiveQuestOne(QuestType.SocialFB, false);

		if(qData == null) {
			return;
		}

		QuestItem questItem = GameResources.Instance.GetQuestData().GetById(qData.QuestId);
		if(questItem.ConditionType == QuestConditionType.InviteFriends) {
			FBController.RequestFriendsList();
			return;
		}

		UpdateQuestInfo(qData);
		ShowQuestInfo(true);
	}

	void ShowQuestInfo(bool animate) {
		if(animate) {
			AnimatedObject ao = QuestInfo.GetComponent<AnimatedObject>();
			ao.AddFadeUI(null, 1, 1).Build().Run();
		} else {
			QuestInfo.GetComponent<CanvasGroup>().alpha = 1;
		}
	}
}
