using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UserData {
	public long Version;
	public int Level;
	public int Experience;
	public int OwnHealth;
	public int OwnDamage;

	public GoodsType DamageEquipmentType = GoodsType.None;
	public GoodsType HealthEquipmentType = GoodsType.None;

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
//	[SerializeField]
	//public List<UserHeroData> HeroesData = new List<UserHeroData>();
//	[SerializeField]
	//public List<QuestData> QuestsData = new List<QuestData>();

	public void InitTest() {
	//	HeroesData.Clear();
		/*
		HeroesData.Add(new UserHeroData("redEnvelop",0));
		HeroesData.Add(new UserHeroData("yellowEnvelop",0));
		HeroesData.Add(new UserHeroData("greenEnvelop",0));		
		HeroesData.Add(new UserHeroData("blueEnvelop",0));
		HeroesData.Add(new UserHeroData("purpleEnvelop",0));

		HeroesData.Add(new UserHeroData("BombH",0));
		HeroesData.Add(new UserHeroData("BombV",0));		
		HeroesData.Add(new UserHeroData("BombP",0));
		HeroesData.Add(new UserHeroData("BombC",0));

		QuestsData.Clear();
		QuestsData.Add(new QuestData("1", 1));
		QuestsData.Add(new QuestData("2", 2));
		*/
		//Experience = 100;
	}

	public void Init() {
		foreach(UserAssetData data in assets) {
			data.Init();
		}

		DamageEquipmentType = GoodsType.Espander;
		HealthEquipmentType = GoodsType.Karti;

	}

	public void InitDefalt() {
		Version = 2;
		Level = 1;
		Experience = 1000;
		OwnHealth = 100;
		OwnDamage = 10;
		
		assets.Add(new UserAssetData(UserAssetType.Money, 5));
		assets.Add(new UserAssetData(UserAssetType.Energy, 100));
		assets.Add(new UserAssetData(UserAssetType.Ring, 10));
		assets.Add(new UserAssetData(UserAssetType.Star, 0));
		assets.Add(new UserAssetData(UserAssetType.Mobile, 0));
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
}
