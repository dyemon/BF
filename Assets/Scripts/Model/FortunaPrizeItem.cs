using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FortunaPrizeItem {
	public UserAssetType UserAssetType;
	public string UserAssetTypeAsString;
	int min;
	int max;

	public void Init() {
		if(UserAssetTypeAsString != null) {
			UserAssetType = EnumUtill.Parse<UserAssetType>(UserAssetTypeAsString);
		}
	}

	public bool IsUserAssetPrize() {
		return UserAssetTypeAsString != null;
	}
}
