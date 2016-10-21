using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class Account {
	private static Account instance = new Account();

	public static Account Instance {
		get {return instance;}
	}

	private AccessToken fbToken;

	public AccessToken AccessToken {
		get {return fbToken;}
		set { fbToken = value; }
	}

	public bool IsLogged {
		get {return AccessToken != null;}
	}

}
