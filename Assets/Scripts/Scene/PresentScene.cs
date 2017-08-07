using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class PresentScene : WindowScene, IFBCallback {
	public const string SceneName = "Present";

	public Button friendCheck;
	public GameObject FriendItem;
	public GridLayoutGroup FriendsList;
	public FBController fbController;

	public static List<object> Friends;

	void Start () {
		fbController.RequestFriendsList();

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
		foreach(FBUser user in friends) {
			GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);
			friendGO.name = user.Id;
			Text name = friendGO.transform.Find("Name").GetComponent<Text>();
			name.text = user.Name;
		}
	}

}
