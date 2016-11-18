using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UserData {
	public long Version;
	public int Level;

	[SerializeField]
	private List<UserAssetData> Assets = new List<UserAssetData>();
	[SerializeField]
	public List<UserHeroData> HeroesData = new List<UserHeroData>();
	[SerializeField]
	public List<QuestData> QuestsData = new List<QuestData>();

	public void InitTest() {
		/*
		HeroesData = new UserHeroData[20];

		HeroesData[0] = new UserHeroData("redVBomb", 2);
		HeroesData[1] = new UserHeroData("redHBomb", 1);
		HeroesData[2] = new UserHeroData("redEnvelop", 2);
		HeroesData[3] = new UserHeroData("redHVBomb", 1);

		HeroesData[4] = new UserHeroData("greenVBomb", 1);
		HeroesData[5] = new UserHeroData("greenHBomb", 2);
		HeroesData[6] = new UserHeroData("greenEnvelop", 2);
		HeroesData[7] = new UserHeroData("greenHVBomb", 1);

		HeroesData[8] = new UserHeroData("blueVBomb", 1);
		HeroesData[9] = new UserHeroData("blueHBomb", 1);
		HeroesData[10] = new UserHeroData("blueEnvelop", 2);
		HeroesData[11] = new UserHeroData("blueHVBomb", 1);

		HeroesData[12] = new UserHeroData("yellowVBomb", 1);
		HeroesData[13] = new UserHeroData("yellowHBomb", 1);
		HeroesData[14] = new UserHeroData("yellowEnvelop", 2);
		HeroesData[15] = new UserHeroData("yellowHVBomb", 1);

		HeroesData[16] = new UserHeroData("purpleVBomb", 1);
		HeroesData[17] = new UserHeroData("purpleHBomb", 1);
		HeroesData[18] = new UserHeroData("purpleEnvelop", 2);
		HeroesData[19] = new UserHeroData("purpleHVBomb", 20);
		*/
		HeroesData.Clear();
		HeroesData.Add(new UserHeroData("redVBomb", 2));
		HeroesData.Add(new UserHeroData("redHBomb", 1));
		HeroesData.Add(new UserHeroData("redEnvelop", 2));
		HeroesData.Add(new UserHeroData("redHVBomb", 1));

		HeroesData.Add(new UserHeroData("greenVBomb", 1));
		HeroesData.Add(new UserHeroData("greenHBomb", 2));
		HeroesData.Add(new UserHeroData("greenEnvelop", 2));
		HeroesData.Add(new UserHeroData("greenHVBomb", 1));

		HeroesData.Add(new UserHeroData("blueVBomb", 1));
		HeroesData.Add(new UserHeroData("blueHBomb", 1));
		HeroesData.Add(new UserHeroData("blueEnvelop", 2));
		HeroesData.Add(new UserHeroData("blueHVBomb", 1));
												
		HeroesData.Add(new UserHeroData("yellowVBomb", 1));
		HeroesData.Add(new UserHeroData("yellowHBomb", 1));
		HeroesData.Add(new UserHeroData("yellowEnvelop", 2));
		HeroesData.Add(new UserHeroData("yellowHVBomb", 1));

		HeroesData.Add(new UserHeroData("purpleVBomb", 1));
		HeroesData.Add(new UserHeroData("purpleHBomb", 1));
		HeroesData.Add(new UserHeroData("purpleEnvelop", 2));
		HeroesData.Add(new UserHeroData("purpleHVBomb", 20));

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
}
