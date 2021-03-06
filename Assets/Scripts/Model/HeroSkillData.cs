﻿using System.Collections;
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
	public string PriceTypeAsString;
	public int PriceValue;

	public int Turns = 0;

	public int MinExperience;

	public TileItemType ExcludeColor;

	public int Damage = 0;
	public int Slowdown = 0;
	public int Energy;
	public int Health;

	public int RemainTurns;
	public bool IsStun = true;

	public string ResultText;
	public int Experience;

	public void init() {
		if(!string.IsNullOrEmpty(TypeAsString)) {
			Type = EnumUtill.Parse<HeroSkillType>(TypeAsString);
		}
		if(!string.IsNullOrEmpty(PriceTypeAsString)) {
			PricaType = EnumUtill.Parse<UserAssetType>(PriceTypeAsString);
		}

		if((int)Type < (int)HeroSkillType.Envelop) {
			TileItemType tiType = EnumUtill.Parse<TileItemType>(Type.ToString());
			DropTileItem = new TileItemData(0, -2, tiType);
			DropTileItem.Level = 0;
		}

		if(Turns > 0 && Slowdown > 0) {
			Description = string.Format (Description, Slowdown, Turns);
		} else if(Turns > 0 && Energy > 0) {
			Description = string.Format (Description, Energy, Turns);
		}else if (Turns > 0) {
			Description = string.Format (Description, Turns);
		} else if (Damage > 0) {
			Description = string.Format (Description, Damage);
		} else if (Health > 0) {
			Description = string.Format (Description, Health);
		}
	}
		

	public static HeroSkillType[] DropTileItemEffects = new HeroSkillType[] {HeroSkillType.ExcludeColor};
	public static HeroSkillType[] DamageEffects = new HeroSkillType[] 
		{HeroSkillType.Damage1, HeroSkillType.Damage2, HeroSkillType.Damage3};
	public static HeroSkillType[] SlowdownEffects = new HeroSkillType[] 
		{HeroSkillType.Slowdown1};
	public static HeroSkillType[] StunEffects = new HeroSkillType[] 
		{HeroSkillType.Stun1, HeroSkillType.Stun2};
	public static HeroSkillType[] EnergyEffects = new HeroSkillType[] 
		{HeroSkillType.Energy};
	public static HeroSkillType[] HealthEffects = new HeroSkillType[] 
		{HeroSkillType.Health1, HeroSkillType.Health2};
	public static HeroSkillType[] InvulnerabilityEffects = new HeroSkillType[] 
		{HeroSkillType.Invulnerability};
	public static HeroSkillType[] KillEaterEffects = new HeroSkillType[] 
		{HeroSkillType.KillEater, HeroSkillType.KillAllEaters};
	public static HeroSkillType[] NotFightingSkills = new HeroSkillType[] 
	{HeroSkillType.BombC, HeroSkillType.BombV, HeroSkillType.BombH, HeroSkillType.BombP,
		HeroSkillType.Envelop, HeroSkillType.ExcludeColor, HeroSkillType.Energy};
}
