using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationData {
	public string Name;
	public string Id;
	public string Description;
	public int AccessOrder;
	public LocationLevelData[] LevelData;

	public int LevelsCount {
		get { return LevelData.Length; }
	}
}
