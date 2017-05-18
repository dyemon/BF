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

	public int Damage = 0;
	public int StunRatio = 0;

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

		if(Turns > 0 && StunRatio > 0) {
			Description = string.Format (Description, StunRatio, Turns);
		} else if (Turns > 0) {
			Description = string.Format (Description, Turns);
		} else if (Damage > 0) {
			Description = string.Format (Description, Damage);
		}
	}

	 
	public static HeroSkillType[] DropTileItemEffects = new HeroSkillType[] {HeroSkillType.ExcludeColor};
	public static HeroSkillType[] DamageEffects = new HeroSkillType[] 
		{HeroSkillType.Damage1, HeroSkillType.Damage2, HeroSkillType.Damage3};
	public static HeroSkillType[] StunEffects = new HeroSkillType[] 
		{HeroSkillType.Stun1, HeroSkillType.Stun2, HeroSkillType.Stun3};

}
