using UnityEngine;
using System.Collections;
using System.IO;
using Facebook.Unity;
using Common.Net;
using Common.Net.Http;

public class GameResources {
	private static string currentLevelId = null;
	private static LevelData currentLevelData = null;

	private static UserData userData;

	private static INIParser settings;
	public static INIParser Settings {
		get{return LoadSettings();}
	}

	public static LevelData LoadLevel(string id) {
		if(currentLevelId == null || currentLevelId != id) {
			TextAsset aText = Resources.Load(Path.Combine(Path.Combine("Config", "Level"), id)) as TextAsset;
			Preconditions.NotNull(aText, "Can not load level {0}", id);
			currentLevelData = JsonUtility.FromJson<LevelData>(aText.text);
			currentLevelData.Init();
		}
	
		return currentLevelData;
	}

	public static UserData LoadUserData() {
		if(userData == null) {
			userData = new UserData();
			userData.Init();
		}

		return userData;
	}

	public static void LoadUserDataFromServer(bool showWaitPanel, bool showErrorMessage) {

		AccessToken token = Account.Instance.AccessToken;
		if(token == null) {
			Debug.Log("Access token is null");
		}


	}
		
	public static INIParser LoadSettings() {
		if(settings == null) {
			TextAsset aText = Resources.Load(Path.Combine("Config", "Settings")) as TextAsset;
			Preconditions.NotNull(aText, "Can not load settings");
			settings = new INIParser();
			settings.Open(aText);
		}

		return settings;
	}
}
