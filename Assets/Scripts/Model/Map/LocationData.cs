﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationData {
	public string Name;
	public string Id;
	public string Description;
	public int AccessOrder;
	public LocationLevelData[] LevelData;

	public int LevelsCountActual {
		get { return LevelData.Length; }
	}

	public void Init() {
		foreach(LocationLevelData item in LevelData) {
			item.Init();
		}
	}
}
