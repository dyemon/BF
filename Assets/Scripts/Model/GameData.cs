using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class GameData {
	public static readonly int  NumColumns = 7;
	public static readonly int NumRows = 7;

	public static readonly int PowerPointSuccess = 100; 
	public static readonly int PowerPointByItem = 3; 
	public static readonly int EnemyTurns = 2; 
	
	public IDictionary<string, HeroData> HeroData = new Dictionary<string, HeroData>();

	public GoodsData[] GoodsData = new GoodsData[0];
	public HeroSkillData[] HeroSkillData = new HeroSkillData[0];

	public void Init() {
		/*
		HeroData["redHBomb"] = new HeroData(TileItemType.RedBombH, 5);
		HeroData["redVBomb"] = new HeroData(TileItemType.RedBombV, 5);
		HeroData["redEnvelop"] = new HeroData(TileItemType.RedEnvelop, 50);
		HeroData["redHVBomb"] = new HeroData(TileItemType.RedBombHV, 5);

		HeroData["greenHBomb"] = new HeroData(TileItemType.GreenBombH, 5);
		HeroData["greenVBomb"] = new HeroData(TileItemType.GreenBombV, 5);
		HeroData["greenEnvelop"] = new HeroData(TileItemType.GreenEnvelop, 50);
		HeroData["greenHVBomb"] = new HeroData(TileItemType.GreenBombHV, 5);

		HeroData["blueHBomb"] = new HeroData(TileItemType.BlueBombH, 50);
		HeroData["blueVBomb"] = new HeroData(TileItemType.BlueBombV, 50);
		HeroData["blueEnvelop"] = new HeroData(TileItemType.BlueEnvelop, 50);
		HeroData["blueHVBomb"] = new HeroData(TileItemType.PurpleBombHV, 50);

		HeroData["yellowHBomb"] = new HeroData(TileItemType.YellowBombH, 50);
		HeroData["yellowVBomb"] = new HeroData(TileItemType.YellowBombV, 50);
		HeroData["yellowEnvelop"] = new HeroData(TileItemType.YellowEnvelop, 50);
		HeroData["yellowHVBomb"] = new HeroData(TileItemType.YellowBombHV, 50);

		HeroData["purpleHBomb"] = new HeroData(TileItemType.PurpleBombH, 50);
		HeroData["purpleVBomb"] = new HeroData(TileItemType.PurpleBombV, 50);
		HeroData["purpleEnvelop"] = new HeroData(TileItemType.PurpleEnvelop, 50);
		HeroData["purpleHVBomb"] = new HeroData(TileItemType.PurpleBombHV, 50);

		HeroData["BombH"] = new HeroData(TileItemType.BombH, 3);
		HeroData["BombV"] = new HeroData(TileItemType.BombV, 3);
		HeroData["BombP"] = new HeroData(TileItemType.BombP, 3);
		HeroData["BombC"] = new HeroData(TileItemType.BombC, 3);
		*/
		foreach(GoodsData item in GoodsData) {
			item.Type = (GoodsType)Enum.Parse(typeof(GoodsType), item.TypeAsString);
		}

		foreach(HeroSkillData item in HeroSkillData) {
			item.init();
		}
	}

	public int GetBombRatio(int level) {
	//	Preconditions.Check(level > 0, "Bomb level must be greater 1");
		return (level == 0)? Mathf.Max(NumColumns, NumRows) :  level + 1;
	}
	
	public float GetPowerMultiplier(int itemCount) {
		return 1 + ((int) (itemCount / 6f)) *0.5f;
	}

	public GoodsData GetGoodsData(GoodsType type) {
		foreach(GoodsData item in GoodsData) {
			if(item.Type == type) {
				return item;
			}
		}
			return null;
	}

	public int CalculatePowerPoint(int itemCountSelect, int itemCountBomb, float ratio) {
		return (int)Mathf.Round((itemCountSelect + itemCountBomb) * ratio * GameData.PowerPointByItem);
	}		
		
}
