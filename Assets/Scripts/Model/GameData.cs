using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {

	public IDictionary<string, HeroData> HeroData = new Dictionary<string, HeroData>();

	public void Init() {
		HeroData["redHBomb"] = new HeroData(TileItemType.RedBombH, 2);
		HeroData["greenVBomb"] = new HeroData(TileItemType.GreenBombV, 1);
		HeroData["blueEnvelop"] = new HeroData(TileItemType.BlueEnvelop, 1);
		HeroData["redEnvelop"] = new HeroData(TileItemType.RedEnvelop, 2);


	}

	public int GetBombRatio(int level) {
		Preconditions.Check(level > 0, "Bomb level must be greater 1");
		return level + 1;
	}
}
