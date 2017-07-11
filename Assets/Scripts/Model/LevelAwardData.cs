using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelAwardData {
	public int Experience;
	public AwardItem[] UserAssets;

	private IDictionary<UserAssetType, int> assets = new Dictionary<UserAssetType, int>();

	public void Init() {
		foreach(AwardItem item in UserAssets) {
			assets[EnumUtill.Parse<UserAssetType>(item.TypeAsString)] = item.Value;
		}
	}

	public void Reset() {
		Experience = 0;
		assets.Clear();
	}

	public void IncreaseAsset(UserAssetType type, int value) {
		if(!assets.ContainsKey(type)) {
			assets[type] = 0;	
		}

		assets[type] += value;
	}

	public int GetAsset(UserAssetType type) {
		return assets.ContainsKey(type) ? assets[type] : 0;
	}
}
