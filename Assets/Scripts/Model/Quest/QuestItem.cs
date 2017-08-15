﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestItem {
	public string Id;
	public string Name;
	public string Description;

	public QuestType Type;
	public string TypeAsString;

	public QuestConditionType ConditionType;
	public string ConditionTypeAsString;
	public int ActionCount;

	public int MinExperience;
	public string RequiredQuestId;

	public AwardItem Award;
	public int ExperienceAward = 0;

	public bool ShowProgressInfo;

	public TargetType? TargetType = null;
	public string TargetTypeAsString = null;
	public int ChainLength;

	public void Init() {
		Type = EnumUtill.Parse<QuestType>(TypeAsString);
		ConditionType = EnumUtill.Parse<QuestConditionType>(ConditionTypeAsString);

		if(Award != null && !string.IsNullOrEmpty(Award.TypeAsString)) {
			Award.Type = EnumUtill.Parse<UserAssetType>(Award.TypeAsString);
		}

		if(!string.IsNullOrEmpty(TargetTypeAsString)) {
			TargetType = EnumUtill.Parse<TargetType>(TargetTypeAsString);
		}
	}
}
