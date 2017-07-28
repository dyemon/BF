using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LocationLevelData {
	public string EnemyTypeAsString;
	public EnemyType? EnemyType;
	public bool Hidden;

	public void Init() {
		if(!string.IsNullOrEmpty(EnemyTypeAsString)) {
			EnemyType = EnumUtill.Parse<EnemyType>(EnemyTypeAsString);
		}

	}
}
