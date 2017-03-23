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
	public int EquipmentHealth;
	public int OwnDamage;
	public int EquipmentDamage;
		
	public int Health {
		get { return OwnHealth + EquipmentHealth; }
	}
	public int Damage {
		get { return OwnDamage + EquipmentDamage; }
	}
	
	[SerializeField]
	private List<UserAssetData> Assets = new List<UserAssetData>();
	[SerializeField]
	public List<UserHeroData> HeroesData = new List<UserHeroData>();
	[SerializeField]
	public List<QuestData> QuestsData = new List<QuestData>();

	public void InitTest() {
	/*	HeroesData.Add(new UserHeroData("redEnvelop",0));
		HeroesData.Add(new UserHeroData("yellowEnvelop",0));
		HeroesData.Add(new UserHeroData("greenEnvelop",0));		
		HeroesData.Add(new UserHeroData("blueEnvelop",0));
		HeroesData.Add(new UserHeroData("purpleEnvelop",0));

		HeroesData.Add(new UserHeroData("BombH",0));
		HeroesData.Add(new UserHeroData("BombV",0));		
		HeroesData.Add(new UserHeroData("BombP",0));
		HeroesData.Add(new UserHeroData("BombC",0));
		*/
		QuestsData.Clear();
		QuestsData.Add(new QuestData("1", 1));
		QuestsData.Add(new QuestData("2", 2));
	}

	public void Init() {
		foreach(UserAssetData data in Assets) {
			data.Init();
		}
	}

	public void InitDefalt() {
		Version = 2;
		Level = 1;
		Experience = 0;
		OwnHealth = 100;
		EquipmentHealth = 0;
		OwnDamage = 10;
		EquipmentDamage = 0;
		
		Assets.Add(new UserAssetData(UserAssetType.GoldCoins, 0));
		Assets.Add(new UserAssetData(UserAssetType.SilverCoins, 100));
		Assets.Add(new UserAssetData(UserAssetType.Energy, 20));
		Assets.Add(new UserAssetData(UserAssetType.Brilliants, 0));
		Assets.Add(new UserAssetData(UserAssetType.Keys, 0));
		Assets.Add(new UserAssetData(UserAssetType.Gold, 0));
	}

	public UserAssetData GetAsset(UserAssetType type) {
		foreach(UserAssetData data in Assets) {
			if(type == data.Type) {
				return data;
			}
		}

		UserAssetData newAsset = new UserAssetData(type, 0);
		Assets.Add(newAsset);

		return newAsset;
	}
	
	public int CalculatePowerPoint(int itemCountSelect, int itemCountBomb, float ratio) {
		Mathf.Round((itemCountSelect + itemCountBomb) * ratio * GameData.PowerPointByItem);
	}		
}
