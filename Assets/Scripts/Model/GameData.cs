using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {

	public IDictionary<string, HeroData> HeroData = new Dictionary<string, HeroData>();

	public void Init() {
		HeroData["red"] = new HeroData(TileItemTypeGroup.Red, 2, 2);
		HeroData["green"] = new HeroData(TileItemTypeGroup.Green, 1, 0);
		HeroData["blue"] = new HeroData(TileItemTypeGroup.Blue, 0, 1);


	}
}
