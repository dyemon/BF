using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class Account {
	private static Account instance = new Account();

	public static Account Instance {
		get {return instance;}
	}

	private AccessToken fbToken = null;
	private FBUser fbUser;

	public AccessToken AccessToken {
		get {return fbToken;}
		set { fbToken = value; }
	}
	public FBUser FBUser {
		get {return fbUser;}
		set { fbUser = value; }
	}

	public bool IsLogged {
		get {return AccessToken != null;}
	}

	public string GetUserId() {
		Preconditions.NotNull(fbToken, "Access token is null");
		return fbToken.UserId;
	}
}
