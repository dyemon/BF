using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PriceItem {
	public UserAssetType Type;
	public string TypeAsString;
	public int Value;

	public void Init() {
		Type = EnumUtill.Parse<UserAssetType>(TypeAsString);
	}
}
