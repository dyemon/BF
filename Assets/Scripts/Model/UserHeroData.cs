using UnityEngine;
using System.Collections;

[System.Serializable]
public class UserHeroData {
	public string Id;
	public int Level;

	public UserHeroData(string id, int level) {
		Id = id;
		Level = level;
	}
}
