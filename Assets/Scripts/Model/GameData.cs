using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {
	public static int NumColumns = 7;
	public static int NumRows = 7;

	public IDictionary<string, HeroData> HeroData = new Dictionary<string, HeroData>();

	public void Init() {
		HeroData["redHBomb"] = new HeroData(TileItemType.RedBombH, 2);
		HeroData["redVBomb"] = new HeroData(TileItemType.RedBombV, 2);
		HeroData["redEnvelop"] = new HeroData(TileItemType.RedEnvelop, 2);
		HeroData["redHVBomb"] = new HeroData(TileItemType.RedBombHV, 2);

		HeroData["greenHBomb"] = new HeroData(TileItemType.GreenBombH, 50);
		HeroData["greenVBomb"] = new HeroData(TileItemType.GreenBombV, 50);
		HeroData["greenEnvelop"] = new HeroData(TileItemType.GreenEnvelop, 50);
		HeroData["greenHVBomb"] = new HeroData(TileItemType.GreenBombHV, 50);

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

		HeroData["BombH"] = new HeroData(TileItemType.BombH, 50);
		HeroData["BombV"] = new HeroData(TileItemType.BombV, 50);
		HeroData["BombP"] = new HeroData(TileItemType.BombP, 50);
		HeroData["BombC"] = new HeroData(TileItemType.BombC, 50);
	}

	public int GetBombRatio(int level) {
	//	Preconditions.Check(level > 0, "Bomb level must be greater 1");
		return (level == 0)? Mathf.Max(NumColumns, NumRows) :  level + 1;
	}
}
