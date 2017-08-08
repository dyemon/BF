using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class GiftScene : WindowScene, IFBCallback {
	public const string SceneName = "Gift";

	public Button friendCheck;
	public GameObject FriendItem;
	public GridLayoutGroup FriendsList;
	public FBController fbController;

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
		for(int i = 0; i < 20; i++) {
			GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);

		}
		return;
		foreach(FBUser user in friends) {
			GameObject friendGO = Instantiate(FriendItem, FriendsList.transform);
			friendGO.name = user.Id;

			Text name = friendGO.transform.Find("Name").GetComponent<Text>();
			name.text = user.Name;

			RawImage icon = UnityUtill.FindByName(friendGO.transform, "Icon").GetComponent<RawImage>();
			if(user.PictureUrl != null) {
				GraphUtil.LoadImgFromURL(user.PictureUrl, (t) => {
					icon.texture = t;
				});
			}
		}
	}

}
