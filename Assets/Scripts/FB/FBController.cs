using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

public class FBController : MonoBehaviour {
	public delegate void OnLogin();
	public static event OnLogin onLogin;
	public delegate void OnLogout();
	public static event OnLogout onLogout;

	public GameObject FBCallback;
	private IFBCallback fBCallback;

	private static IList<FBUser> friends;

	void Awake() {
		if(FBCallback != null) {
			fBCallback = FBCallback.GetComponent<IFBCallback>();
		}

		if(!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
		//	FB.ActivateApp();
		}
	}

	private void InitCallback() {
		if(FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
	
		//	FB.getLoginStatus();
			if(FB.IsLoggedIn) {
				Account.Instance.AccessToken = GetAccessToken();
				if(onLogin != null) {
					onLogin();
				}
				GetProfile(true);
				return;
			} 
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");

		}

		Debug.Log("FB IsLogged " + FB.IsLoggedIn);

		if(fBCallback != null) {
			fBCallback.OnFBInit();
		}
	}

	private void OnHideUnity(bool isGameShown) {
		if(!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void OnClickLoginFB() {
		List<string> perms = new List<string>(){ "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions(perms, AuthCallback);


	}

	public void OnClickLogoutFB() {
		FB.LogOut();
		Account.Instance.AccessToken = null;
		Account.Instance.FBUser = null;
		ClearFriendsCache();

		if(onLogout != null) {
			onLogout();
		}

		if(fBCallback != null) {
			fBCallback.OnFBLogout();
		}
	}

	private void AuthCallback(ILoginResult result) {
		string error = null;

		if(FB.IsLoggedIn) {
		//	Dictionary<string, object> res = (Dictionary<string, object>)result.ResultDictionary;
			// AccessToken class will have session details
			Account.Instance.AccessToken = GetAccessToken();
			// Print current access token's User ID
			Debug.Log(Account.Instance.AccessToken.UserId);
			// Print current access token's granted permissions
			foreach(string perm in Account.Instance.AccessToken.Permissions) {
				Debug.Log(perm);
			}

			if(onLogin != null) {
				onLogin();
			}

			GetProfile(false);
		} else {
			Debug.Log("User cancelled login");
			Account.Instance.AccessToken = null;
			if(result != null && !string.IsNullOrEmpty(result.Error)) {
				error = result.Error;
			}

			if(fBCallback != null) {
				fBCallback.OnFBLoginFail(error);
			}
		}
	}

	public AccessToken GetAccessToken() {
		return Facebook.Unity.AccessToken.CurrentAccessToken;
	}



	public void InviteFriends () {
		// Prompt user to send a Game Request using FB.AppRequest
		// https://developers.facebook.com/docs/unity/reference/current/FB.AppRequest
		FB.AppRequest(
			"Присоединяйся к нам",
			null,
			null,
			null,
			null,
			null,
			"Пригласи друзей",
			InviteFriendsCallback
		);
	}

	// Callback for FB.AppRequest
	private void InviteFriendsCallback (IAppRequestResult result) {
		// Error checking
		Debug.Log("AppRequestCallback");
		if (result.Error != null) {
			Debug.LogError(result.Error);
			ModalPanels.Show(ModalPanelName.ErrorPanel, string.Format("Ошибка при запросе к facebook \n {0}", result.Error));
			return;
		}
		Debug.Log(result.RawResult);

		// Check response for success - show user a success popup if so
		object obj;
		if (result.ResultDictionary.TryGetValue ("cancelled", out obj)) {
			DisplayMessageController.DisplayMessage("Запрос отменён", Color.red);
		}
		else if (result.ResultDictionary.TryGetValue ("request", out obj)){
			DisplayMessageController.DisplayMessage("Приглашение отправлено", Color.green);
		}
	}

	public bool RequestFriendsList() {
		if(friends != null) {
			fBCallback.OnFriendsRequest(friends);
			return true;
		}

	//	FB.API("/me/invitable_friends?fields=id,name,picture.width(130).height(130)&limit=100", HttpMethod.GET, FriendsListCallback);
		FB.API("/me/friends?fields=id,name,picture.width(130).height(130)&limit=100", HttpMethod.GET, FriendsListCallback);
	//	FB.API("/me/apprequests", HttpMethod.GET, FriendsListCallback);

		return false;
	}

	void FriendsListCallback(IGraphResult result) {
		if(result.Error != null) {
			ModalPanels.Show(ModalPanelName.ErrorPanel, string.Format("Ошибка при запросе к facebook \n {0}", result.Error));
			return;
		}

		ModalPanels.Show(ModalPanelName.ErrorPanel, "Update friends cache");

		Debug.Log("hi= " + result.RawResult);
		object dataList;
		friends = new List<FBUser>();

		if(result.ResultDictionary.TryGetValue("data", out dataList)) {
			var friendsList = (List<System.Object>)dataList;
			//	CacheFriends(friendsList);
			Debug.Log("friendsList= " + friendsList );

			for(int i = 0; i < friendsList.Count; i++) {
				friends.Add(new FBUser(friendsList[i]));
			}
		}

		GameTimers.Instance.StartClearFrendsCache();
		fBCallback.OnFriendsRequest(friends);
	}


	void GetProfile(bool isInit) {
	//	string queryString = "/me?fields=id,name,picture.width(120).height(120)";
	//	FB.API(queryString, HttpMethod.GET, GetProfileCallbackAuth);
		string queryString = "/me";

		FB.API(queryString, HttpMethod.GET, (r) => { if(isInit) GetProfileCallbackInit(r); else  GetProfileCallbackAuth(r);});
	}

	void GetProfileCallbackInit(IGraphResult result) {
		if(GetProfileCallback(result) && fBCallback != null) {
			fBCallback.OnFBInit();
		}
	}

	void GetProfileCallbackAuth(IGraphResult result) {
		if(GetProfileCallback(result) && fBCallback != null) {
			fBCallback.OnFBLoginSuccess();
		}
	}


	bool GetProfileCallback(IGraphResult result) {
		Debug.Log("GetProfileCallback");
		if (result.Error != null){
			if(fBCallback != null) {
					fBCallback.OnFBLoginFail(result.Error);
			}
			return false;
		}

	//	Debug.Log(result.RawResult);

		FBUser user = new FBUser(result.ResultDictionary);
		Account.Instance.FBUser = user;
		GameResources.Instance.GetLocalData().FBUser = user;
		GameResources.Instance.SaveLocalData();

		return true;
	
	}

	public static void ClearFriendsCache() {
		friends = null;
	}
}
