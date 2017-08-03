using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class InviteFriends : WindowScene {
	public const string SceneName = "InviteFriends";

	public Button friendCheck;
	public GameObject FriendItem;
	public GridLayoutGroup FriendsList;
	public FBController fbController;

	void Start () {
		fbController.RequestFriendsList(FriendsListCallback);

	}
	

	void FriendsListCallback(IGraphResult result) {
		if(result.Error != null) {
			ModalPanels.Show(ModalPanelName.ErrorPanel, string.Format("Ошибка при запросе к facebook \n {0}", result.Error));
			return;
		}


	}
}
