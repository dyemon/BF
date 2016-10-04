﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {

	public IDictionary<string, HeroData> HeroData = new Dictionary<string, HeroData>();

	public void Init() {
		HeroData["redHBomb"] = new HeroData(TileItemType.RedBombH, 50);
		HeroData["redVBomb"] = new HeroData(TileItemType.RedBombV, 50);
		HeroData["redEnvelop"] = new HeroData(TileItemType.RedEnvelop, 50);

		HeroData["greenHBomb"] = new HeroData(TileItemType.GreenBombH, 50);
		HeroData["greenVBomb"] = new HeroData(TileItemType.GreenBombV, 50);
		HeroData["greenEnvelop"] = new HeroData(TileItemType.GreenEnvelop, 50);

		HeroData["blueHBomb"] = new HeroData(TileItemType.BlueBombH, 50);
		HeroData["blueVBomb"] = new HeroData(TileItemType.BlueBombV, 50);
		HeroData["blueEnvelop"] = new HeroData(TileItemType.BlueEnvelop, 50);

		HeroData["yellowHBomb"] = new HeroData(TileItemType.YellowBombH, 50);
		HeroData["yellowVBomb"] = new HeroData(TileItemType.YellowBombV, 50);
		HeroData["yellowEnvelop"] = new HeroData(TileItemType.YellowEnvelop, 50);

		HeroData["purpleHBomb"] = new HeroData(TileItemType.PurpleBombH, 50);
		HeroData["purpleVBomb"] = new HeroData(TileItemType.PurpleBombV, 50);
		HeroData["purpleEnvelop"] = new HeroData(TileItemType.PurpleEnvelop, 50);
	}

	public int GetBombRatio(int level) {
		Preconditions.Check(level > 0, "Bomb level must be greater 1");
		return level + 1;
	}
}
