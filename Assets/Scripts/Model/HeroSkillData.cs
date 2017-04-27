using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroSkillData {
	public HeroSkillType Type;
	public string TypeAsString;
	public string Name;
	public string Description;

	public TileItemData DropTileItem;
	public int Count = 1;

	public UserAssetType PricaType;
	public string PricaTypeAsString;
	public int PriceValue;

	public int Turns = 0;
	public int RemainTurns;

	public int MinExperience;

	public TileItemType ExcludeColor;

	public void init() {
		if(!string.IsNullOrEmpty(TypeAsString)) {
			Type = EnumUtill.Parse<HeroSkillType>(TypeAsString);
		}
		if(!string.IsNullOrEmpty(PricaTypeAsString)) {
			PricaType = EnumUtill.Parse<UserAssetType>(PricaTypeAsString);
		}

		if((int)Type < (int)HeroSkillType.Envelop) {
			TileItemType tiType = EnumUtill.Parse<TileItemType>(Type.ToString());
			DropTileItem = new TileItemData(0, -2, tiType);
			DropTileItem.Level = 0;
		}

		if(Turns > 0) {
			Description = string.Format(Description, Turns);
		}
	}


}
