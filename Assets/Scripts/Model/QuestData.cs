using UnityEngine;
using System.Collections;

[System.Serializable]
public class QuestData {
	public QuestData(string id, int progress) {
		Id = id;
		Progress = progress;
	}

	public string Id;
	public int Progress;
}
