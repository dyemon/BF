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

		uiMoveType.Add(UIMoveType.AWARD_EXPERIENCE, 0.5f);

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

	private static int currentLevel = 1;
	private static int currentLocation = 1;
	private static int currentCity = 1;
	private static int currentLocationLevel = 1;

	public static int CurrentLevel {
		get { return currentLevel; }
		set {currentLevel = value;}
	}
	public static int CurrentLocation {
		get { return currentLocation; }
		set {currentLocation = value;}
	}	
	public static int CurrentCity {
		get { return currentCity; }
		set {currentCity = value;}
	}	
	public static int CurrentLocationLevel {
		get { return currentLocationLevel; }
		set {currentLocationLevel = value;}
	}	

	public static void InitLocationParams(UserData uData) {
		LocalSettingsData localData = GameResources.Instance.GetLocalSettings();
		int level = localData.LastLevel;

		CurrentLevel = level;
		Vector3 param = GameResources.Instance.GetMapData().GetLocation(level);
		CurrentCity = (int)param.x;
		CurrentLocation = (int)param.y;
		CurrentLocationLevel = (int)param.z;
	}
}
