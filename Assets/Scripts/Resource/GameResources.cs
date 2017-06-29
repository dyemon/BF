using UnityEngine;
using System.Collections;
using System.IO;
using Facebook.Unity;
using Common.Net;
using Common.Net.Http;

public class GameResources {
	public static GameResources Instance = new GameResources();

	public delegate void OnUpdateUserAsset(UserAssetType type, int value);
	public event OnUpdateUserAsset onUpdateUserAsset;

	private int currentLevelId = 0;
	private LevelData currentLevelData = null;
	private GameData gameData = null;
	private LocalSettingsData localSettings = null;

	private string userData;

	private INIParser settings;
	public INIParser Settings {
		get{return LoadSettings();}
	}

	private string getKey() {
		string res = "eydh47dj439548kis1)+&";
		return res;
	}

	public LevelData GetLevel(int id) {
		if(currentLevelId == 0 || currentLevelId != id) {
			TextAsset aText = Resources.Load(Path.Combine(Path.Combine("Config", "Level"), id.ToString())) as TextAsset;
			Preconditions.NotNull(aText, "Can not load level {0}", id);
			currentLevelData = JsonUtility.FromJson<LevelData>(aText.text);
			currentLevelData.Init();
			currentLevelId = id;
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
			uData.Init();
	//	if(Application.isEditor) {
				uData.InitTest();
	//		}
			saveUserDataLocal(uData);
		}
	}

	public GameData GetGameData() {
		if(gameData == null) {
			TextAsset aText = Resources.Load(Path.Combine("Config", "Game")) as TextAsset;
			Preconditions.NotNull(aText, "Can not load Game data");
			gameData = JsonUtility.FromJson<GameData>(aText.text);
			gameData.Init();
		}
		return gameData;
	}
	
	private void saveUserDataLocal(UserData userData) {
		string json = JsonUtility.ToJson(userData);
		this.userData = B64X.Encode(json);
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
		uData.Init();
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
			data.Init();
			if(Application.isEditor) {
				data.InitTest();
			}
			this.userData = B64X.Encode((JsonUtility.ToJson(data)));
			SaveUserData(userData, false);
		} else if(data.Version < userData.Version) {
			SaveUserDataToServer(userData);
		}
	}	

	public void SaveUserData(UserData data, bool saveToServer) {
		if(data == null) {
			data = GetUserData();
		} else {
			saveUserDataLocal(data);
		}

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
		

	public bool ChangeUserAsset(UserAssetType type, int value) {
		UserData userData = GetUserData();
		if(!ChangeUserAsset(userData, type, value)) {
			return false;
		}
			
		return true;
	}


	public bool ChangeUserAsset(UserData data, UserAssetType type, int value) {
		UserAssetData asset = data.GetAsset(type);
		int newVal = asset.Value + value;
		if(newVal < 0) {
			return false;
		}
		asset.Value = newVal;
		saveUserDataLocal(data);

		if(onUpdateUserAsset != null) {
			onUpdateUserAsset(type, value);
		}

		return true;
	}

	public bool CanChangeAsset(UserData data, UserAssetType type, int value) {
		UserAssetData asset = data.GetAsset(type);
		return asset.Value + value >= 0;
	}

	public LocalSettingsData GetLocalSettings() {
		if(localSettings == null) {
			string data = PlayerPrefs.GetString("localSettings");
			localSettings = JsonUtility.FromJson<LocalSettingsData>(data);
			if(localSettings == null) {
				localSettings = new LocalSettingsData();
			}
		}

		return localSettings;
	}

	public void SaveLocalSettings() {
		string data = JsonUtility.ToJson(localSettings);
		PlayerPrefs.SetString("localSettings", data);
		PlayerPrefs.Save();
	}

	public bool Buy(UserAssetType type, int count) {
		if(type != UserAssetType.Money) {
			GameData gData = GetGameData();
			int val = gData.GetPriceValue(type);
			
			if(!ChangeUserAsset(UserAssetType.Money, -val * count)) {
				return false;
			}
		}

		ChangeUserAsset(type, count);
	//	SaveUserData(null, type == UserAssetType.Money);

		return true;
	}

	public bool Buy(HeroSkillData skill) {
		if(!ChangeUserAsset(skill.PricaType, -skill.PriceValue)) {
			SceneController.Instance.ShowUserAssetsScene(skill.PricaType, true);
			return false;
		}
		SaveUserData(null, false);
		return true;
	}

	public void IncreaseTileItemCollect(TileItemType type, int level) {
		if(type != TileItemType.Star) {
			return;
		}

		UserData data = GetUserData();
		data.IncreaseTileItemCollect(level, type);

		saveUserDataLocal(data);
	}
}
