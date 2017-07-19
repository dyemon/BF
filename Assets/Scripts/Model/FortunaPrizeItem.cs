using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FortunaPrizeItem {
	public UserAssetType UserAssetType;
	public string UserAssetTypeAsString;
	public int min;
	public int max;

	public void Init() {
		if(UserAssetTypeAsString != null) {
			UserAssetType = EnumUtill.Parse<UserAssetType>(UserAssetTypeAsString);
		}
	}

	public bool IsUserAssetPrize() {
		return UserAssetTypeAsString != null;
	}

	public int GetPrizeAmount() {
		return Random.Range(min, max);
	}
}
