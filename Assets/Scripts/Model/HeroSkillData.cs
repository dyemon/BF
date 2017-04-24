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

	public void init() {
		if(!string.IsNullOrEmpty(TypeAsString)) {
			Type = EnumUtill.Parse<HeroSkillType>(TypeAsString);
		}
			
		if(TypeGroup == HeroSkillTypeGroup.DropTileItem) {
			TileItemType tiType = EnumUtill.Parse<TileItemType>(Type.ToString());
			DropTileItem = new TileItemData(0, -2, tiType);
			DropTileItem.Level = 0;
		}
	}

	public HeroSkillTypeGroup TypeGroup {
		get {
			return (HeroSkillTypeGroup)((int)Type - (int)Type % 100);
		}
	}
}
