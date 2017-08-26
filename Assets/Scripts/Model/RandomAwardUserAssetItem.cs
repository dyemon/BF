using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomAwardUserAssetItem {
	public UserAssetType Type;
	public string TypeAsString;
	public int Ratio;
	public int Min;
	public int Max;

	public void Init() {
		Type = EnumUtill.Parse<UserAssetType>(TypeAsString);
	}
}
