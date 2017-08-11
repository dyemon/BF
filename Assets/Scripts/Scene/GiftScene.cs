using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using Common.Net.Http;

public class GiftScene : WindowScene, IFBCallback {
	public const string SceneName = "Gift";

	public Toggle SelectAll;
	public GameObject FriendItem;
	public GridLayoutGroup FriendsList;
	public FBController fbController;
	public InputField Filter;
	public ScrollRect FriendsScrollRect;

	private string friendItemTag = "FriendItem";

	public static List<object> Friends;

	void Start () {
	//	fbController.RequestFriendsList();
		OnFriendsRequest(null);
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
		string filter = Filter.text;
		UnityUtill.DestroyByTag(FriendsList.transform, friendItemTag);

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

		foreach(FBUser user in friends) {
			if(!string.IsNullOrEmpty(filter) && user.Name.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) < 0) {
				continue;
			}

			GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);
			friendGO.name = user.Id;
			friendGO.transform.tag = friendItemTag;

			Text nameText = friendGO.transform.Find("Name").GetComponent<Text>();
			nameText.text = user.Name;

			RawImage icon = UnityUtill.FindByName(friendGO.transform, "Icon").GetComponent<RawImage>();
			if(user.PictureUrl != null) {
				GraphUtil.LoadImgFromURL(user.PictureUrl, (t) => {
					icon.texture = t;
				});
			}

			GameObject mark = friendGO.transform.Find("Mark").gameObject;
			friendGO.GetComponent<Button>().onClick.AddListener(() => {
				mark.SetActive(!mark.activeSelf);
			});
		}
	}

	public void OnFilterChanged() {
		OnFriendsRequest(null);
	}

	public void ClearFilter() {
		Filter.text = "";
		OnFriendsRequest(null);
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
		string sendIds = "";

		foreach(Transform item in FriendsList.transform) {
			if(item.transform.tag != friendItemTag) {
				continue;
			}

			GameObject mark = item.Find("Mark").gameObject;
			if(!mark.activeSelf) {
				continue;
			}

			string delimeter = string.IsNullOrEmpty(sendIds)? "" : ","; 
			sendIds += delimeter + item.gameObject.name;
			Destroy(item.gameObject);
		}

		FriendsScrollRect.verticalNormalizedPosition = 1;
		Debug.Log(sendIds);

	//	HttpRequest request = new HttpRequest().Url(HttpRequester.URL_SEND_GIFT)
		//	.Success(onSuccess).Error(onError)
		//	.ShowWaitPanel(showWaitPanel)
		//	.Param("userId", userId);
	}
}
