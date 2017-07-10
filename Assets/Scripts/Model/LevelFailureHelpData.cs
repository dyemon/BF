using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelFailureHelpData {
	public int Health;
	public int Turns;
	public string PriceTypeAsString;
	public int PriceValue;
	public UserAssetType PriceType;

	public void Init() {
		PriceType = EnumUtill.Parse<UserAssetType>(PriceTypeAsString);
	}
}
