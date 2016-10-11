using UnityEngine;
using System.Collections;
using System.IO;

public class GameResources {
	private static string currentLevelId = null;
	private static LevelData currentLevelData = null;

	public static LevelData LoadLevel(string id) {
		if(currentLevelId == null || currentLevelId != id) {
			TextAsset aText = Resources.Load(Path.Combine(Path.Combine("Config", "Level"), id)) as TextAsset;
			Preconditions.NotNull(aText, "Can not load level {0}", id);
			currentLevelData = JsonUtility.FromJson<LevelData>(aText.text);
			currentLevelData.Init();
		}
	
		return currentLevelData;
	}
}
