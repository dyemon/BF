using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestProgressData {
	public QuestProgressData(string questId, QuestType type) {
		QuestId = questId;
		Type = type;
	}

	public QuestType Type;
	public string QuestId;
	public int Progress;
	public bool IsComplete;
	public bool IsTakenAward;
}
