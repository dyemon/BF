using UnityEngine;
using System.Collections;
using System.IO;
using Facebook.Unity;
using Common.Net;
using Common.Net.Http;

public class GameResources {
	public static GameResources Instance = new GameResources();

	private string currentLevelId = null;
	private LevelData currentLevelData = null;

	private string userData;

	private INIParser settings;
	public INIParser Settings {
		get{return LoadSettings();}
	}

	private string getKey() {
		string res = "eydh47dj439548kis1)+&";
		return res;
	}

	public LevelData LoadLevel(string id) {
		if(currentLevelId == null || currentLevelId != id) {
			TextAsset aText = Resources.Load(Path.Combine(Path.Combine("Config", "Level"), id)) as TextAsset;
			Preconditions.NotNull(aText, "Can not load level {0}", id);
			currentLevelData = JsonUtility.FromJson<LevelData>(aText.text);
			currentLevelData.Init();
		}
	
		return currentLevelData;
	}

	public void LoadUserData() {
		if(this.userData == null) {
			UserData uData;
			string data = PlayerPrefs.GetString("data");
			if(string.IsNullOrEmpty(data)) {
				uData = initDefaltUserData();
			} else {
				string json = StringCipher.Decrypt(data, getKey());
				uData = JsonUtility.FromJson<UserData>(json);
			}
			if(Application.isEditor) {
				uData.Init();
			}
			this.userData = B64X.Encode((JsonUtility.ToJson(uData)));
		}
	}

	private UserData initDefaltUserData() {
		UserData data = new UserData();
		data.InitDefalt();
		return data;
	}

	public UserData GetUserData() {
		//clearUserData();
		LoadUserData();
		UserData uData = JsonUtility.FromJson<UserData>(B64X.Decode(this.userData));
		return uData;
	}

	private void clearUserData() {
		PlayerPrefs.SetString("data", "");
		PlayerPrefs.Save();
	}

	public void MergeUserData(UserData data) {
		Preconditions.NotNull(data, "User data for merge is null");
		UserData userData = GetUserData();

		if(data.Version > userData.Version) {
			if(Application.isEditor) {
				data.Init();
			}
			this.userData = B64X.Encode((JsonUtility.ToJson(data)));
			SaveUserData(false);
		} else if(data.Version < userData.Version) {
			SaveUserDataToServer(userData);
		}
	}	

	public void SaveUserData(bool saveToServer) {
		UserData data = GetUserData();

		if(saveToServer) {
			SaveUserDataToServer(data);
		}
			
		data.Version++;
		string json = JsonUtility.ToJson(data);
		string encData = StringCipher.Encrypt(json, getKey());
		PlayerPrefs.SetString("data", encData);
		PlayerPrefs.Save();
	}


	public void SaveUserDataToServer(UserData data) {
		if(!Account.Instance.IsLogged) {
			return;
		}

		Preconditions.NotNull(data, "Can not save user data. User data is null");

		HttpRequest request = new HttpRequest().Url(HttpRequester.URL_USER_SAVE)
			.PostData(JsonUtility.ToJson(data));

		HttpRequester.Instance.Send(request);
		
	}

	public void LoadUserDataFromServer(string userId, bool showWaitPanel, HttpRequest.OnSuccess onSuccess, HttpRequest.OnError onError) {
		HttpRequest request = new HttpRequest().Url(HttpRequester.URL_USER_LOAD)
			.Success(onSuccess).Error(onError)
			.ShowWaitPanel(showWaitPanel)
			.Param("userId", userId);

		HttpRequester.Instance.Send(request);
	}

	public INIParser LoadSettings() {
		if(settings == null) {
			TextAsset aText = Resources.Load(Path.Combine("Config", "Settings")) as TextAsset;
			Preconditions.NotNull(aText, "Can not load settings");
			settings = new INIParser();
			settings.Open(aText);
		}

		return settings;
	}
}
