using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FortunaData {
	public FortunaPrizeItem[] Prizes;
	public FortunaPrizeItem[] Jackpot;

	public void Init() {
		foreach(FortunaPrizeItem item in Prizes) {
			item.Init();
		}

		foreach(FortunaPrizeItem item in Jackpot) {
			item.Init();
		}
	}

	public FortunaPrizeItem GetItem(UserAssetType type) {
		foreach(FortunaPrizeItem item in Prizes) {
			if(item.IsUserAssetPrize() && item.UserAssetType == type) {
				return item;
			}
		}

		return null;
	}
}
