using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

public class FBController : MonoBehaviour {
	public GameObject FBCallback;
	private IFBCallback fBCallback;

	void Awake() {
		if(FBCallback != null) {
			fBCallback = FBCallback.GetComponent<IFBCallback>();
		}

		if(!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	private void InitCallback() {
		if(FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
			Account.Instance.AccessToken = GetAccessToken();

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

		if(fBCallback != null) {
			fBCallback.OnFBLogout();
		}
	}

	private void AuthCallback(ILoginResult result) {
		string error = null;

		if(FB.IsLoggedIn) {
			// AccessToken class will have session details
			Account.Instance.AccessToken = GetAccessToken();
			// Print current access token's User ID
			Debug.Log(Account.Instance.AccessToken.UserId);
			// Print current access token's granted permissions
			foreach(string perm in Account.Instance.AccessToken.Permissions) {
				Debug.Log(perm);
			}

			if(fBCallback != null) {
				fBCallback.OnFBLoginSuccess();
			}
		} else {
			Debug.Log("User cancelled login");
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
}
