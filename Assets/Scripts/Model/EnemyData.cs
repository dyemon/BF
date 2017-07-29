using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EnemyData {
	//public EnemyType Type;
	//public string TypeAsString;
	public int Health;
	public int Damage;
	public int SkillRatio;
	public EnemySkillData[] SkillData;

	public void Init() {
	//	Type = EnumUtill.Parse<EnemyType>(TypeAsString);

		if(SkillData != null) {
			foreach(EnemySkillData item in SkillData) {
				item.Type = (EnemySkillType)Enum.Parse(typeof(EnemySkillType), item.TypeAsString);
			}
		}
	}
}
