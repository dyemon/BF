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
			item.Type = EnumUtill.Parse<UserAssetType>(item.TypeAsString);
			assets[item.Type] = item.Value;
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

	public AwardItem GetRandomAwardItem() {
		return UserAssets.Length > 0? UserAssets[Random.Range(0, UserAssets.Length)] : null;
	}
}
