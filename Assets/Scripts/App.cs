using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class App {
	public static readonly float moveTileItemDelay = 0.0f;
	public static readonly bool IsShowHttpError = true;
	public static readonly float SlimeAnimationTime = 0.2f;

	private static IDictionary<TileItemMoveType, float> tileItemMoveType = new Dictionary<TileItemMoveType, float>();

	private static float moveTileItemTimeUnit = 0f;

	static App() {
		tileItemMoveType.Add(TileItemMoveType.DOWN, 10f);
		tileItemMoveType.Add(TileItemMoveType.OFFSET, 10f);
		tileItemMoveType.Add(TileItemMoveType.MIX, 30f);
		tileItemMoveType.Add(TileItemMoveType.HERO_DROP, 10f);
		tileItemMoveType.Add(TileItemMoveType.GENERATED_TILEITEM_DROP, 5f);
		tileItemMoveType.Add(TileItemMoveType.EATER, 10f);
		tileItemMoveType.Add(TileItemMoveType.HERO_SKILL, 15f);
		tileItemMoveType.Add(TileItemMoveType.BUY_USERASSET, 1000f);
		tileItemMoveType.Add(TileItemMoveType.AWARDTILEITEM_1, 150f);
		tileItemMoveType.Add(TileItemMoveType.AWARDTILEITEM_2, 150f);
	}

	public static float MoveTileItemTimeUnit {
		get {
			if(moveTileItemTimeUnit == 0f) {
				moveTileItemTimeUnit = AMove.CalcTime(Vector3.zero, new Vector3(0, 1, 0), tileItemMoveType[TileItemMoveType.DOWN]);
			}

			return moveTileItemTimeUnit;
		}
	}

	public static float GetTileItemSpeed(TileItemMoveType type) {
		return tileItemMoveType[type];
	}

	public static int GetCurrentLevel() {
		return 1004;
	}
		
}
