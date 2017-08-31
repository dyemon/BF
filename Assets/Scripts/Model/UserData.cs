using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class UserData {
	public long Version;
	public int Level;
	public int Experience;
	public int OwnHealth;
	public int OwnDamage;

	public GoodsType DamageEquipmentType = GoodsType.None;
	public GoodsType HealthEquipmentType = GoodsType.None;

	public long StartTimestamp;
	public long LastSavedTimestamp;

	public long StartInfinityEnergyTimestamp;
	public int InfinityEnergyDuration;
	public int DailyBonus;
	public bool DailyBonusTaken;

	public long FortunaLastTry;
	public int FortunaTryCount;

	public int Health {
		get { 
			GoodsData data = GameResources.Instance.GetGameData().GetGoodsData(HealthEquipmentType);
			return OwnHealth + ((data != null)? data.Health : 0); 
		}
	}
	public int Damage {
		get { 
			GoodsData data = GameResources.Instance.GetGameData().GetGoodsData(DamageEquipmentType);
			return OwnDamage  + ((data != null)? data.Damage : 0); 
		}
	}

	[SerializeField]
	private List<LevelSaveData> levelData = new List<LevelSaveData>();
	[SerializeField]
	private List<UserAssetData> assets = new List<UserAssetData>();
	[SerializeField]
	private List<QuestProgressData> quests = new List<QuestProgressData>();
	[SerializeField]
	private List<UserKachalkaItem> kachalkaData = new List<UserKachalkaItem>();

	[SerializeField]
	private string fbSendedGiftUserIds;
	[SerializeField]
	private long fbSendedGiftTimestamp;
	[SerializeField]
	private string fbReceivedGiftUserIds;

	public void Init() {
		foreach(UserAssetData data in assets) {
			data.Init();
		}
	}

	public void InitDefalt() {
		Version = 2;
		Level = 1;
		Experience = 1100;
		OwnHealth = 100;
		OwnDamage = 10;
		
		assets.Add(new UserAssetData(UserAssetType.Money, 5));
		assets.Add(new UserAssetData(UserAssetType.Energy, 20));
		assets.Add(new UserAssetData(UserAssetType.Ring, 10));
		assets.Add(new UserAssetData(UserAssetType.Star, 0));
		assets.Add(new UserAssetData(UserAssetType.Mobile, 20));

		StartTimestamp = DateTimeUtill.ConvertToUnixTimestamp(DateTime.Now);
		LastSavedTimestamp = StartTimestamp;

	//	StartInfinityEnergyTimestamp = 0;
		InfinityEnergyDuration = 0;
		DailyBonus = 1;
		DailyBonusTaken = false;

		ResetFortunaTryCount();

		GameTimers.Instance.Init(this);
		App.InitLocationParams(this);
	//	Level = 50;
	}

	public void InitOnStart() {
		InitTimestampOnStart();
		GameTimers.Instance.Init(this);
	//	Experience = 100;
//		for(int i = 0; i < 1000; i++) {
//			fbReceivedGiftUserIds += "152536263325262,";
//		}
		//	Level = 60;
		App.InitLocationParams(this);
	}

	public void Merge(UserData uData) {
		InitTimestampOnStart();
		if(InfinityEnergyDuration > 0) {
			GameTimers.Instance.StartInfinityEnergyTimer();
		}
		App.InitLocationParams(this);
	}

	public void InitTimestampOnStart() {
		long currentTimestamp = GetCurrentTimestamp();

		if(DateTimeUtill.IsYesterday(currentTimestamp, StartTimestamp)) {
			DailyBonusTaken = false;
		} else if(!DateTimeUtill.IsToday(currentTimestamp, StartTimestamp)) {
			DailyBonusTaken = false;
			DailyBonus = 1;
		}

		if(!DailyBonusTaken) {
			ParametersController.Instance.SetParameter(ParametersController.CANSEL_DAILYBONUS, false);
		}

		StartTimestamp = currentTimestamp;

		if(InfinityEnergyDuration > 0) {
			InfinityEnergyDuration -= (int)Math.Round((StartTimestamp - LastSavedTimestamp)/60f);
		}
		if(InfinityEnergyDuration <= 0) {
			ResetInfinityEnergy();
		}

		GameData gData = GameResources.Instance.GetGameData();
		EnergyData eData = gData.EnergyData;

		UserAssetData energy = GetAsset(UserAssetType.Energy);
		if(energy.Value < eData.MaxIncreaseCount) {
			energy.Value +=  (int)Math.Floor((StartTimestamp - LastSavedTimestamp)/(60f * eData.IncreaseTimeOffline));
			if(energy.Value > eData.MaxIncreaseCount) {
				energy.Value = eData.MaxIncreaseCount;
			}
		}


	}

	public UserAssetData GetAsset(UserAssetType type) {
		foreach(UserAssetData data in assets) {
			if(type == data.Type) {
				return data;
			}
		}

		UserAssetData newAsset = new UserAssetData(type, 0);
		assets.Add(newAsset);

		return newAsset;
	}

	private LevelSaveData GetLevelData(int level) {
		for(int i = levelData.Count - 1; i >= 0; i--) {
			if(levelData[i].Level == level) {
				return levelData[i];
			}
		}

		LevelSaveData data = new LevelSaveData();
		data.Level = level;
		levelData.Add(data);
		return data;
	}

	public bool IncreaseTileItemCollect(int level, TileItemType type) {
		if(type != TileItemType.Star) {
			return false;
		}

		LevelSaveData data = GetLevelData(level);
		data.StarCollectCount++;
		return true;
	}

	public bool CheckTileItemCollect(int level, TileItemType type) {
		if(type != TileItemType.Star) {
			return true;
		}

		LevelSaveData data = GetLevelData(level);
		return data.StarCollectCount == 0;
	}

	public void OnSuccessLevel(int level) {
		LevelSaveData data = GetLevelData(level);
		data.SuccessCount++;
	}

	public int GetSuccessCount(int level) {
		LevelSaveData data = GetLevelData(level);
		return data.SuccessCount;
	}

	public long GetCurrentTimestamp() {
		long now = DateTimeUtill.ConvertToUnixTimestamp(DateTime.Now);
		LocalData lData = GameResources.Instance.GetLocalData();
		return now > lData.SrvTime ? now : lData.SrvTime;
	}

	public void AddInfinityEnergy(int duration) {
		InfinityEnergyDuration += duration;
	}

	public void ResetInfinityEnergy() {
	//	StartInfinityEnergyTimestamp = 0;
		InfinityEnergyDuration = 0;
	}

	public void DecreaseInfinityEnergy(int value) {
		InfinityEnergyDuration -= value;
		if(InfinityEnergyDuration <= 0) {
			ResetInfinityEnergy();
		}
	}

	public void UpdateLastSaved() {
		LastSavedTimestamp = GetCurrentTimestamp();
	}

	public void ResetFortunaTryCount() {
		GameData gData = GameResources.Instance.GetGameData();
		FortunaTryCount = gData.FortunaData.TryCount;
	}

	public QuestProgressData GetQuestById(string id) {
		foreach(QuestProgressData item in quests) {
			if(item.QuestId == id) {
				return item;
			}
		}

		return null;
	}

	public bool IsCompleteQuest(string id) {
		QuestProgressData qpData = GetQuestById(id);
		if(qpData == null) {
			return false;
		}
		if(qpData.IsComplete) {
			return true;
		}

		QuestItem qItem = GameResources.Instance.GetQuestData().GetById(id);
		qpData.IsComplete = qItem.ActionCount <= qpData.Progress;
		return qpData.IsComplete;
	}

	public bool IsTakenQuestAward(string id) {
		QuestProgressData qpData = GetQuestById(id);
		if(qpData == null) {
			return false;
		}
		return qpData.IsTakenAward;
	}

	public bool UpDateQuests(QuestType? type) {
		QuestData qData = GameResources.Instance.GetQuestData();
		bool res = false;

		foreach(QuestItem item in qData.QuestItemsData) {
			if(type != null && type != item.Type) {
				continue;
			}

			if(item.MinExperience > Experience) {
				continue;
			}

			if(item.RequiredQuestId != null) {
				QuestProgressData qpData = GetQuestById(item.RequiredQuestId);
				if(qpData == null || !qpData.IsTakenAward) {
					continue;
				}
			}

			QuestProgressData qp = GetQuestById(item.Id);
			if(qp == null) {
				quests.Add(new QuestProgressData(item.Id, item.Type));
				res = true;
			}
		}

		if(res) {
			GameResources.Instance.SaveUserData(this, false);
		}

		return res;
	}

	public QuestProgressData GetActiveQuestOne(QuestType? type, bool notCompleteOnly) {
		IList<QuestProgressData> quests = GetActiveQuests(type, notCompleteOnly);
		Preconditions.Check(quests.Count <= 1, "Active {0} quest count = {1}", new System.Object[] {type.Value, quests.Count});
		return quests.Count == 0 ? null : quests[0];
	}

	public IList<QuestProgressData> GetActiveQuests(QuestType? type, bool notCompleteOnly) {
		IList<QuestProgressData> res = new List<QuestProgressData>();

		foreach(QuestProgressData item in quests) {
			if(type != null && type != item.Type) {
				continue;	
			}
			if(item.IsTakenAward) {
				continue;
			}

			if(!item.IsComplete || (!notCompleteOnly && !item.IsTakenAward)) {
				res.Add(item);
			}
		}

		return res;
	}
		
	public string[] GetSendedGiftUserIds() {
		if(!DateTimeUtill.IsToday(GetCurrentTimestamp(), fbSendedGiftTimestamp)) {
			fbSendedGiftUserIds = "";
		}

		return string.IsNullOrEmpty(fbSendedGiftUserIds) ? new String[0] : fbSendedGiftUserIds.Split(',');
	}

	public void UpdateSendedGift(List<string> ids) {
		string[] newIds = GetSendedGiftUserIds().Concat(ids).ToArray();
		if(newIds.Length > 400) {
			newIds = newIds.Take(0).Concat(newIds.Skip(newIds.Length - 400)).ToArray();
		}
		fbSendedGiftUserIds = string.Join(",", newIds);
		fbSendedGiftTimestamp = GetCurrentTimestamp();
	}

	public void AddReceivedGiftUserIds(string ids) {
		if(string.IsNullOrEmpty(ids)) {
			return;
		}

		if(fbReceivedGiftUserIds != "") {
			fbReceivedGiftUserIds += ",";
		}

		fbReceivedGiftUserIds += ids;
		if(fbReceivedGiftUserIds.Length > 2000) {
			fbReceivedGiftUserIds = fbReceivedGiftUserIds.Substring(0, 2000);
		}
	}

	public void TakeGift(string id) {
		if(string.IsNullOrEmpty(fbReceivedGiftUserIds) || string.IsNullOrEmpty(id)) {
			return;
		}

		string[] rids = fbReceivedGiftUserIds.Split(',');
		int numIndex = Array.IndexOf(rids, id);
		if(numIndex < 0) {
			return;
		}

		rids = rids.Where((val, idx) => idx != numIndex).ToArray();
		fbReceivedGiftUserIds = string.Join(",", rids);
	}

	public void TakeAllGifts() {
		fbReceivedGiftUserIds = "";
	}

	public string[] GetReceivedGiftUserIds() {
		return string.IsNullOrEmpty(fbReceivedGiftUserIds) ? new string[0] : fbReceivedGiftUserIds.Split(',');
	}

	public void RemoveReceivedGiftIds(IList<string> ids) {
		string[] newIds = GetReceivedGiftUserIds().
			Where((val, idx) => !ids.Contains(val)).ToArray();

		fbReceivedGiftUserIds = string.Join(",", newIds);
	}

	public UserKachalkaItem GetKachalkaItem(KachalkaType type) {
		foreach(UserKachalkaItem item in kachalkaData) {
			if(item.Type == type) {
				return item;
			}
		}

		return null;
	}

	public bool IncreaseKachalkaIndex(KachalkaType type) {
		bool res = false;

		UserKachalkaItem item = GetKachalkaItem(type);
		if(item == null) {
			item = new UserKachalkaItem();
			item.Type = type;
			kachalkaData.Add(item);
		}

		KachalkaData kData = GameResources.Instance.GetGameData().GetKachalkaData(type);
		Preconditions.NotNull(kData, "KachalkaData is null for type: {0}", type);

		KachalkaItem kItem = kData.Items[item.ItemIndex];
		if(kItem.Steps.Length - 2 > item.StepIndex) {
			item.StepIndex++;
		} else {
			item.ItemIndex++;
			item.StepIndex = -1;
			OwnHealth += kItem.Health;
			OwnDamage += kItem.Damage;
			res = true;
		}
		return res;
	}

	public bool IsKachalkaAvaliable() {
		foreach(KachalkaData kData in GameResources.Instance.GetGameData().KachalkaDataItems) {
			int itemIndex = 0;
			int stepIndex = -1;
			UserKachalkaItem item = GetKachalkaItem(kData.Type);
			if(item != null) {
				itemIndex = item.ItemIndex;
				stepIndex = item.StepIndex;
			}

			KachalkaItem kItem = kData.Items[itemIndex];
			if(stepIndex >= kItem.Steps.Length - 1) {
				itemIndex++;
			}
			if(itemIndex >= kData.Items.Length - 1) {
				continue;
			}

			if(kData.Items[itemIndex].MinExperience <= Experience) {
				return true;
			}
		}

		return false;
	}

	public bool IsGoodsAvaliable() {
		GameData gameData = GameResources.Instance.GetGameData();

		GoodsData userHealth = gameData.GetGoodsData(HealthEquipmentType);
		GoodsData userDamage = gameData.GetGoodsData(DamageEquipmentType);

		foreach(GoodsData goodsData in gameData.GoodsData) {
			bool isHealth = goodsData.Health > 0;

			if(isHealth && userHealth != null && userHealth.Health >= goodsData.Health) {
				continue;
			}
			if(!isHealth && userDamage != null && userDamage.Damage >= goodsData.Damage) {
				continue;
			}

			return goodsData.MinExperience <= Experience;
		}
		return false;
	}
}
