using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetController : MonoBehaviour {
	private LevelData levelData;

	public Dictionary<TargetType, Sprite> TargetSprites = new Dictionary<TargetType, Sprite>();
	public int T;

	public void Init () {
		levelData = GameResources.LoadLevel(App.GetCurrentLevel());

		foreach(TargetData data in levelData.TargetData) {
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
