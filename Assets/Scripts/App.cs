using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class App {
	public static readonly float moveTileItemDelay = 0.0f;
	public static readonly bool IsShowHttpError = true;

	private static IDictionary<TileItemMoveType, float> tileItemMoveType = new Dictionary<TileItemMoveType, float>();

	private static float moveTileItemTimeUnit = 0f;

	static App() {
		tileItemMoveType.Add(TileItemMoveType.DOWN, 10f);
		tileItemMoveType.Add(TileItemMoveType.OFFSET, 15f);
		tileItemMoveType.Add(TileItemMoveType.MIX, 30f);
		tileItemMoveType.Add(TileItemMoveType.HERO_DROP, 10f);
		tileItemMoveType.Add(TileItemMoveType.GENERATED_TILEITEM_DROP, 5f);
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

	public static string GetCurrentLevel() {
		return "2_test";
	}
}
