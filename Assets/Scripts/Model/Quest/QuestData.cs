using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData {
	public QuestItem[] QuestItemsData;

	public void Init() {
		foreach(QuestItem item in QuestItemsData) {
			item.Init();
		}
	}

	public QuestItem GetById(string id) {
		foreach(QuestItem item in QuestItemsData) {
			if(item.Id == id) {
				return item;
			}
		}

		return null;
	}

	public QuestItem Get(QuestType type, QuestConditionType condType) {
		foreach(QuestItem item in QuestItemsData) {
			if(item.Type == type && item.ConditionType == condType ) {
				return item;
			}
		}

		return null;
	}
}
