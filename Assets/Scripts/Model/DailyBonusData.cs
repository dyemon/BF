using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyBonusData {
	public AwardItem GreatestBonus;
	public AwardItem[] DailyBonuses;

	public void Init() {
		foreach(AwardItem item in DailyBonuses) {
			item.Type = EnumUtill.Parse<UserAssetType>(item.TypeAsString);
		}

		GreatestBonus.Type =  EnumUtill.Parse<UserAssetType>(GreatestBonus.TypeAsString);
	}
}
