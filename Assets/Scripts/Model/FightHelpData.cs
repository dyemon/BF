using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FightHelpData {
	public int ConditionDamageRatio;
	public int ConditionHealthRatio;
	public int IncreaceDamageRatio;
	public int IncreaceHealthRatio;
	public int PriceValue;
	public string PriceTypeAsString;
	public UserAssetType PriceType;

	public void Init() {
		PriceType = EnumUtill.Parse<UserAssetType>(PriceTypeAsString);
	}
}
