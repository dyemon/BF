using UnityEngine;
using System.Collections;

public class App {

	public static readonly float moveTileItemSpeed = 9f;
	public static readonly float moveTileItemDelay = 0.0f;

	private static float moveTileItemTimeUnit = 0f;
	public static float MoveTileItemTimeUnit {
		get {
			if(moveTileItemTimeUnit == 0f) {
				moveTileItemTimeUnit = AMove.CalcTime(Vector3.zero, new Vector3(0, 1, 0), moveTileItemSpeed);
			}

			return moveTileItemTimeUnit;
		}
	}


}
