using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController {
	public static readonly QuestController Instance = new QuestController();

	public bool IsComplete(string id) {
		UserData uData = GameResources.Instance.GetUserData();
		QuestProgressData qpData = uData.GetQuestById(id);
		if(qpData == null) {
			return false;
		}
		if(qpData.IsComplete) {
			return true;
		}

		QuestItem qItem = GameResources.Instance.GetQuestData().GetById(id);
		return qItem.ActionCount <= qpData.Progress;
	}


}
