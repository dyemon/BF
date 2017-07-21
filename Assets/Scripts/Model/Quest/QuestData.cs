using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData {
	public QuestItem[] QuestItemsData;

	private IDictionary<string, QuestItem> quests = new Dictionary<string, QuestItem>();

	public void Init() {
		foreach(QuestItem item in QuestItemsData) {
			item.Init();
			quests.Add(item.Id, item);
		}
	}

	public QuestItem GetById(string id) {
		QuestItem item;
		quests.TryGetValue(id, out item);
		return item;
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
