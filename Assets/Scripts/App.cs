using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class App {
	public static readonly float moveTileItemDelay = 0.0f;
	public static readonly bool IsShowHttpError = true;
	public static readonly float SlimeAnimationTime = 0.2f;

	private static IDictionary<TileItemMoveType, float> tileItemMoveType = new Dictionary<TileItemMoveType, float>();
	private static IDictionary<UIMoveType, float> uiMoveType = new Dictionary<UIMoveType, float>();

	private static float moveTileItemTimeUnit = 0f;

	static App() {
		tileItemMoveType.Add(TileItemMoveType.DOWN, 10f);
		tileItemMoveType.Add(TileItemMoveType.OFFSET, 10f);
		tileItemMoveType.Add(TileItemMoveType.MIX, 30f);
		tileItemMoveType.Add(TileItemMoveType.HERO_DROP, 10f);
		tileItemMoveType.Add(TileItemMoveType.GENERATED_TILEITEM_DROP, 5f);
		tileItemMoveType.Add(TileItemMoveType.EATER, 10f);
		tileItemMoveType.Add(TileItemMoveType.HERO_SKILL, 15f);

		uiMoveType.Add(UIMoveType.BUY_USERASSET, 0.7f);
		uiMoveType.Add(UIMoveType.AWARDTILEITEM_1, 0.2f);
		uiMoveType.Add(UIMoveType.AWARDTILEITEM_2, 0.5f);
		uiMoveType.Add(UIMoveType.AWARD_EXPERIENCE_1, 0.7f);
		uiMoveType.Add(UIMoveType.AWARD_EXPERIENCE_2, 0.5f);
		uiMoveType.Add(UIMoveType.FAILURE_AWARD_1, 0.3f);
		uiMoveType.Add(UIMoveType.FAILURE_AWARD_2, 0.5f);
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

	public static float GetMoveTime(UIMoveType type) {
		return uiMoveType[type];
	}

	public static int GetCurrentLevel() {
		return 1005;
	}
		
}
