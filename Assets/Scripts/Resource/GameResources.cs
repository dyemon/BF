using UnityEngine;
using System.Collections;
using System.IO;
using Facebook.Unity;
using Common.Net;
using Common.Net.Http;
using System.Collections.Generic;
using SimpleJSON;

public class GameResources {
	public static GameResources Instance = new GameResources();

	public delegate void OnUpdateUserAsset(UserAssetType? type, int value);
	public event OnUpdateUserAsset onUpdateUserAsset;

	public delegate void OnUpdateInfinityEnergy(int value);
	public event OnUpdateInfinityEnergy onUpdateInfinityEnergy;

	public delegate void OnUpdateExperience(int value);
	public event OnUpdateExperience onUpdateExperience;

	public delegate void OnCompleteQuest(QuestItem quest);
	public event OnCompleteQuest onCompleteQuest;

	public delegate void OnCheckGift(string[] ids);
	public event OnCheckGift onCheckGift;

	private int currentLevelId = 0;
	private LevelData currentLevelData = null;
	private GameData gameData = null;
	private LocalData localData = null;
	private QuestData questData;
	private MapData mapData;

	private string userData;
	private long prevVersion = -1;

	public GameResources() {
	}

	public void AddEventListeners() {
		HttpRequester.Instance.AddEventListener(HttpRequester.URL_CHECK_GIFT, OnCheckGiftHttp);
	}

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
			if(aText == null) {
				return null;
			}
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
				uData.Init();
			} else {
				string json = StringCipher.Decrypt(data, getKey());
				uData = JsonUtility.FromJson<UserData>(json);
				uData.Init();
				uData.InitOnStart();
			}
				
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

	public QuestData GetQuestData() {
		if(questData == null) {
			TextAsset aText = Resources.Load(Path.Combine("Config", "Quest")) as TextAsset;
			Preconditions.NotNull(aText, "Can not load Quest data");
			questData = JsonUtility.FromJson<QuestData>(aText.text);
			questData.Init();
		}
		return questData;
	}

	public MapData GetMapData() {
		if(mapData == null) {
			TextAsset aText = Resources.Load(Path.Combine("Config", "Map")) as TextAsset;
			Preconditions.NotNull(aText, "Can not load Map data");
			mapData = JsonUtility.FromJson<MapData>(aText.text);
			mapData.Init();
		}
		return mapData;
	}

	private void saveUserDataLocal(UserData userData) {
		userData.UpdateLastSaved();
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

	public bool MergeUserData(UserData data) {
		Preconditions.NotNull(data, "User data for merge is null");
		UserData userData = GetUserData();

		if(data.Version > userData.Version && data.Level >= userData.Level) {
			data.Init();
			LocalData lData = GetLocalData();
			lData.LastLevel = data.Level;
			SaveLocalData();

			data.Merge(userData);

			this.userData = B64X.Encode((JsonUtility.ToJson(data)));
			SaveUserData(null, false);
			return true;
		} else if(data.Version < userData.Version) {
			SaveUserDataToServer(userData);
		}
	
		return false;
	}	

	public void SaveUserData(UserData data, bool saveToServer) {
		if(data == null) {
			data = GetUserData();
		} 
			
		data.UpdateLastSaved();
		data.Version++;
		saveUserDataLocal(data);

		if(saveToServer) {
			SaveUserDataToServer(data);
		}
			
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

		if(prevVersion == data.Version) {
			return;
		}
		prevVersion = data.Version;

		HttpRequest request = new HttpRequest(HttpRequester.URL_USER_SAVE)
			.HttpMethod("POST").Param("data", JsonUtility.ToJson(data));
	
		HttpRequester.Instance.Send(request);	
	}

	public void LoadUserDataFromServer(bool showWaitPanel) {
		HttpRequest request = new HttpRequest(HttpRequester.URL_USER_LOAD)
			.ShowWaitPanel(showWaitPanel);

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

	public bool ChangeUserAsset(IList<AwardItem> awards) {
		UserData data = GetUserData();
		bool updateExp = false;
		bool updateAssets = false;

		foreach(AwardItem item in awards) {
			if(item.Experience > 0) {
				data.Experience += item.Experience;
				updateExp = true;
			} else {
				if(item.Type == UserAssetType.Energy && item.Value < 0 && data.InfinityEnergyDuration > 0) {
					continue;
				}
				UserAssetData asset = data.GetAsset(item.Type);
				int newVal = asset.Value + item.Value;
				if(newVal < 0) {
					return false;
				}
				asset.Value = newVal;
				updateAssets = true;
			}
		}

		saveUserDataLocal(data);

		if(updateAssets && onUpdateUserAsset != null) {
			onUpdateUserAsset(null, 0);
		}

		if(updateExp && onUpdateExperience != null) {
			onUpdateExperience(data.Experience);
		}

		return true;
	}

	public bool ChangeUserAsset(UserData data, UserAssetType type, int value) {
		if(type == UserAssetType.Energy && value < 0 && data.InfinityEnergyDuration > 0) {
			return true;
		}

		UserAssetData asset = data.GetAsset(type);
		int newVal = asset.Value + value;
		if(newVal < 0) {
			return false;
		}
		asset.Value = newVal;
		saveUserDataLocal(data);

		if(onUpdateUserAsset != null) {
			onUpdateUserAsset(type, asset.Value);
		}

		return true;
	}
	/*
	public bool CanChangeAsset(UserData data, UserAssetType type, int value) {
		UserAssetData asset = data.GetAsset(type);
		return asset.Value + value >= 0;
	}
	*/
	public LocalData GetLocalData() {
		if(localData == null) {
			string data = PlayerPrefs.GetString("localSettings");
			if(string.IsNullOrEmpty(data)) {
				localData = new LocalData();
			} else {
				string json = StringCipher.Decrypt(data, getKey());
				localData = JsonUtility.FromJson<LocalData>(json);
			}
		}

		return localData;
	}

	public void SaveLocalData() {
		string data = JsonUtility.ToJson(localData);
		string encData = StringCipher.Encrypt(data, getKey());
		PlayerPrefs.SetString("localSettings", encData);
		PlayerPrefs.Save();
	}

	public void IncreaseTileItemCollect(TileItemType type, int level) {
		if(type != TileItemType.Star) {
			return;
		}

		UserData data = GetUserData();
		data.IncreaseTileItemCollect(level, type);

		saveUserDataLocal(data);
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

	public bool Buy(GoodsData data) {
		UserData uData = GetUserData();

		if(!ChangeUserAsset(uData, data.PriceType, -data.PriceValue)) {
			SceneController.Instance.ShowUserAssetsScene(data.PriceType, true);
			return false;
		}

		if(data.Health > 0) {
			uData.HealthEquipmentType = data.Type;
		} else {
			uData.DamageEquipmentType = data.Type;
		}

		saveUserDataLocal(uData);
		return true;
	}

	public bool BuyInfinityEnergy(int count) {
		GameData gData = GetGameData();
		UserData uData = GetUserData();

		if(!ChangeUserAsset(uData, UserAssetType.Money, -gData.EnergyData.InfinityPrice * count)) {
			return false;
		}

		uData.AddInfinityEnergy(count * 60);
		saveUserDataLocal(uData);

		if(onUpdateInfinityEnergy != null) {
			onUpdateInfinityEnergy(uData.InfinityEnergyDuration);
		}

		return true;
	}

	public bool DecreaseInfinityEnergy(int val) {
		UserData uData = GetUserData();
		uData.DecreaseInfinityEnergy(val);
		saveUserDataLocal(uData);

		FireUpdateInfinityEnergy(uData);


		return uData.InfinityEnergyDuration > 0;
	}

	public void IncreaseExperience(int exp) {
		UserData uData = GetUserData();
		uData.Experience += exp;
		saveUserDataLocal(uData);

		if(onUpdateExperience != null) {
			onUpdateExperience(uData.Experience);
		}
	}

	public void LevelSuccess(int level) {
		UserData uData = GetUserData();
	//	Preconditions.Check(level <= uData.Level, "Level {0} grater then user current level {1}", level, uData.Level);
		uData.OnSuccessLevel(level);
		if(level == uData.Level) {
			uData.Level++;
			LocalData locData = GetLocalData();
			locData.LastLevel = uData.Level;
			SaveLocalData();
		}
		saveUserDataLocal(uData);
	}

	public void IncrementDailyBonus() {
		UserData uData = GetUserData();
		uData.DailyBonus++;
		uData.DailyBonusTaken = true;
		saveUserDataLocal(uData);
	}

	public int DecreaseFortunaTryCount(int count) {
		UserData uData = GetUserData();
		uData.FortunaTryCount -= count;
		if(uData.FortunaTryCount <= 0) {
			uData.FortunaTryCount = 0;
			uData.FortunaLastTry = uData.GetCurrentTimestamp();
			GameTimers.Instance.StartFortunaTimer(0);
		}
		saveUserDataLocal(uData);
		return uData.FortunaTryCount;
	}

	public void ResetFortunaTryCount() {
		UserData uData = GetUserData();
		uData.ResetFortunaTryCount();
		saveUserDataLocal(uData);
	}

	public QuestProgressData IncreasQuestAction(string id, int count, bool save, bool setProgress = false, bool resetProgress = false) {
		UserData uData = GetUserData();
		QuestProgressData qData = Preconditions.NotNull(uData.GetQuestById(id), "Quest with id {0} not activate", id);
		if(setProgress) {
			qData.Progress = count;
		} else {
			qData.Progress += count;
		}
		bool prevIscomplete = qData.IsComplete;
		QuestItem qItem = GetQuestData().GetById(id);
		if(qData.Progress >= qItem.ActionCount) {
			qData.IsComplete = true;
		} else if(resetProgress) {
			qData.IsComplete = false;
		}
		if(save) {
			SaveUserData(uData, false);
		} else {
			saveUserDataLocal(uData);
		}

		if(!prevIscomplete && qData.IsComplete && onCompleteQuest != null) {
			onCompleteQuest(qItem);
		}

		return qData;
	}

	public void TakeQuestAward(string id) {
		UserData uData = GetUserData();
		QuestProgressData qData = Preconditions.NotNull(uData.GetQuestById(id), "Quest with id {0} not activate", id);

		qData.IsTakenAward = true;
		uData.UpDateQuests(qData.Type);

		saveUserDataLocal(uData);
	}

	public void FireUpdateInfinityEnergy(UserData uData) {
		if(uData == null) {
			uData = GetUserData();
		}

		if(onUpdateInfinityEnergy != null) {
			onUpdateInfinityEnergy(uData.InfinityEnergyDuration);
		}
	}

	public void UpdateSendedGift(List<string> ids) {
		UserData uData = GetUserData();
		uData.UpdateSendedGift(ids);
		saveUserDataLocal(uData);
	}

	public bool CheckGift() {
		bool updated = ParametersController.Instance.GetBool(ParametersController.RECEIVED_GIFT_CACHE_UPDATED);
		if(updated) {
			return false;
		}
	//	ModalPanels.Show(ModalPanelName.MessagePanel, "Update gift received");
		ParametersController.Instance.SetParameter(ParametersController.RECEIVED_GIFT_CACHE_UPDATED, true);
		GameTimers.Instance.StarGiftCach();

		HttpRequest request = new HttpRequest(HttpRequester.URL_CHECK_GIFT); 
		HttpRequester.Instance.Send(request);
		return true;
	}

	void OnCheckGiftHttp(HttpResponse response) {
		
		string data = response.GetParameter("data");
		if(string.IsNullOrEmpty(data)) {
			return;
		}

		JSONNode root = JSON.Parse(data);
		if(root == null) {
			return;
		}
		string ids = "";
		foreach(JSONObject item in root.AsArray) {
			if(ids != "") {
				ids += ",";
			}
			ids += item["values"]["socialId"];
		}

		if(ids == "") {
			return;
		}

		UserData uData = GetUserData();
		uData.AddReceivedGiftUserIds(ids);
		SaveUserData(uData, false);

		if(onCheckGift != null) {
			onCheckGift(uData.GetReceivedGiftUserIds());
		}


	}

	public void TakeGift(string id) {
		UserData uData = GetUserData();
		uData.TakeGift(id);
		saveUserDataLocal(uData);
	}
		
	public void TakeAllGifts() {
		UserData uData = GetUserData();
		uData.TakeAllGifts();
		saveUserDataLocal(uData);
	}

	public void RemoveReceivedGiftIds(IList<string> ids) {
		UserData uData = GetUserData();
		uData.RemoveReceivedGiftIds(ids);
		saveUserDataLocal(uData);
	}

	public bool IncreaseKachalkaIndex(KachalkaType type) {
		UserData uData = GetUserData();
		bool res = uData.IncreaseKachalkaIndex(type);
		saveUserDataLocal(uData);
		return res;
	}


}
