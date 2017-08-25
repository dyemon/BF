using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using Common.Net.Http;
using System.Linq;

public class GiftScene : WindowScene, IFBCallback {
	public enum Type {
		Send, Receive
	}

	public const string SceneName = "Gift";

	public Toggle SelectAll;
	public GameObject FriendItem;
	public GridLayoutGroup FriendsList;
	public FBController fbController;
	public InputField Filter;
	public ScrollRect FriendsScrollRect;

	public GameObject SendButton;
	public GameObject ReceiveButton;
	public GameObject SendToggle;
	public GameObject ReceiveToggle;
	public Text Title;

	private string friendItemTag = "FriendItem";

	private Type currentSceneType = Type.Send;

	public static List<object> Friends;

	private IDictionary<string, Texture> userImageCache = new Dictionary<string, Texture>();
	private List<string> sendedIds = new List<string>();
	private bool save;

	void OnEnable() {
		HttpRequester.Instance.AddEventListener(HttpRequester.URL_SEND_GIFT, OnSuccessSendGift);
		GameResources.Instance.onCheckGift += OnCheckGift;
		save = false;
	}



	void OnDisable() {
		HttpRequester.Instance.RemoveEventListener(HttpRequester.URL_SEND_GIFT, OnSuccessSendGift);
		GameResources.Instance.onCheckGift -= OnCheckGift;
		if(save) {
			GameResources.Instance.SaveUserData(null, false);
		}
	}

	void OnCheckGift (string[] ids) {
		fbController.RequestFriendsList();
	}

	void Start () {
		if(SceneController.Instance != null) {
			currentSceneType = (Type)SceneController.Instance.GetParameter(SceneName);
		}

		UpdateSceneItems();
		fbController.RequestFriendsList();
	//	OnFriendsRequest(null);
	}
	
	public void OnFBInit() {
	}

	public void OnFBLoginSuccess() {
	}

	public void OnFBLogout() {
	}
		
	public void OnFBLoginFail(string error) {
	}

	public void OnFriendsRequest(IList<FBUser> friends) {
		if(currentSceneType == Type.Receive && GameResources.Instance.CheckGift()) {
			return;
		}

		UnityUtill.DestroyByTag(FriendsList.transform, friendItemTag);
		FriendsScrollRect.verticalNormalizedPosition = 1;

		/*
		for(int i = 0; i < 20; i++) {
			string name = "Имя " + i;
			if(!string.IsNullOrEmpty(filter) && name.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) < 0) {
				continue;
			}

			GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);
		//	friendGO.transform.SetParent(FriendsList.transform);
			friendGO.transform.tag = friendItemTag;
			friendGO.name = i.ToString();

			Text nameText = friendGO.transform.Find("Name").GetComponent<Text>();
			nameText.text = name;

			GameObject mark = friendGO.transform.Find("Mark").gameObject;
			friendGO.GetComponent<Button>().onClick.AddListener(() => {
				mark.SetActive(!mark.activeSelf);
			});
			if(SelectAll.isOn) {
				mark.SetActive(true);
			}

		}
		return;
		*/
		UserData uData = GameResources.Instance.GetUserData();
		string[] ids = currentSceneType == Type.Send?  uData.GetSendedGiftUserIds() : uData.GetReceivedGiftUserIds();
		int i = 0;
		foreach(FBUser user in friends) {
 
			if(!AddUser(user, ids)) {
				continue;
			}
			i++;
			if(i >= 30) {
				break;
			}
		}


	}

	bool AddUser(FBUser user, string[] ids) {
		string filter = Filter.text;

		if(!string.IsNullOrEmpty(filter) && user.Name.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) < 0) {
			return false;
		}
		if(currentSceneType == Type.Send && ids.Contains(user.Id)) {
			return false;
		}
	
		GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);
		friendGO.name = user.Id;
		friendGO.transform.tag = friendItemTag;

		Text nameText = friendGO.transform.Find("Name").GetComponent<Text>();
		nameText.text = user.Name;

		if(user.PictureUrl != null) {
			RawImage icon = UnityUtill.FindByName(friendGO.transform, "Icon").GetComponent<RawImage>();

			Texture tx = null;
			userImageCache.TryGetValue(user.PictureUrl, out tx);
			if(tx != null) {
				icon.texture = tx;
			} else {
				GraphUtil.LoadImgFromURL(user.PictureUrl, (t) => {
					if(icon != null) {
						icon.texture = t;
					} 
					userImageCache[user.PictureUrl] = t;
				});
			}
		}

		GameObject mark = friendGO.transform.Find("Mark").gameObject;
		mark.SetActive(currentSceneType == Type.Send);

		GameObject gift = friendGO.transform.Find("Gift").gameObject;
		gift.SetActive(currentSceneType == Type.Receive);
	
		if(currentSceneType == Type.Send) {
			friendGO.GetComponent<Button>().onClick.AddListener(() => {
				mark.SetActive(!mark.activeSelf);
			});
			if(SelectAll.isOn) {
				mark.SetActive(true);
			}
		} else {
			friendGO.GetComponent<Button>().onClick.AddListener(() => {
				OnTakeGift(friendGO);
			});
		}


		return true;
	}

	public void OnFilterChanged() {
		fbController.RequestFriendsList();
	}

	public void ClearFilter() {
		Filter.text = "";
		fbController.RequestFriendsList();
	}

	public void OnSelectAll(bool state) {
		foreach(Transform item in FriendsList.transform) {
			if(item.transform.tag != friendItemTag) {
				continue;
			}

			GameObject mark = item.Find("Mark").gameObject;
			mark.SetActive(state);
		}
	}

	public void OnSend() {
		sendedIds.Clear();

		foreach(Transform item in FriendsList.transform) {
			if(item.transform.tag != friendItemTag) {
				continue;
			}

			GameObject mark = item.Find("Mark").gameObject;
			if(!mark.activeSelf) {
				continue;
			}
				
			sendedIds.Add(item.gameObject.name);
		//	Destroy(item.gameObject);
		}

		if(sendedIds.Count == 0) {
			return;
		}
			
		Debug.Log(sendedIds);

		HttpRequest request = new HttpRequest(HttpRequester.URL_SEND_GIFT)
			.ShowWaitPanel(true).ShowErrorMessage(true)
			.Param("socialIdTo", string.Join(",", sendedIds.ToArray()));

		HttpRequester.Instance.Send(request);
	}

	void OnSuccessSendGift(HttpResponse response) {
		GameResources.Instance.UpdateSendedGift(sendedIds);
		save = true;
		fbController.RequestFriendsList();
	}

	void UpdateSceneItems() {
		bool isSend = currentSceneType == Type.Send;

		SelectAll.gameObject.SetActive(isSend);
		SendButton.SetActive(isSend);
		ReceiveButton.SetActive(!isSend);
		SendToggle.SetActive(!isSend);
		ReceiveToggle.SetActive(isSend);

		Title.text = isSend ? "Отправить подарок" : "Получить подарок";
	}

	public void ToggleSceneType(bool isSend) {
		currentSceneType = isSend ? Type.Send : Type.Receive;
		UpdateSceneItems();
		fbController.RequestFriendsList();
	}

	void OnTakeGift(GameObject friendGO) {
	}
}
