using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UserData {
	public string UserId;
	public long Version;
	public MainUserData main;
	public UserHeroData[] HeroesData;
	public QuestData[] QuestsData;

	public void Init() {
		
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

	}

	public void InitDefalt() {
		Version = 1;
		main = new MainUserData();
		main.Level = 1;
		main.Brilliants = 0;
		main.GoldCoins = 0;
		main.SilverCoins = 100;
		main.Energy = 20;
		main.Keys = 0;
		main.Gold = 0;

		HeroesData = new UserHeroData[0];
		QuestsData = new QuestData[0];
	}
}
