using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IFBCallback {
	void OnFBInit();
	void OnFBLoginSuccess();
	void OnFBLogout();
	void OnFBLoginFail(string error);
	void OnFriendsRequest(IList<FBUser> friends);
}
