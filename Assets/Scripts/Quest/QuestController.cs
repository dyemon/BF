using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController {
	public static readonly QuestController Instance = new QuestController();

	public delegate void OnQuestUpdate();
	public event OnQuestUpdate onQuestUpdate;

	void FireQuestUpdate() {
		if(onQuestUpdate != null) {
			onQuestUpdate();
		}
	}

	public void OnCollectTileItem(TileItem ti) {
		if(ti == null) {
			return;
		}

		UserData uData = GameResources.Instance.GetUserData();
		QuestData qData = GameResources.Instance.GetQuestData();

		foreach(QuestProgressData pd in uData.GetActiveQuests(QuestType.Game, true)) {
			QuestItem qi = qData.GetById(pd.QuestId);
			if(qi.ConditionType == QuestConditionType.Collect && 
				TargetController.EqualsByType(ti, qi.TargetType.Value)) {
				GameResources.Instance.IncreasQuestAction(pd.QuestId, 1, false);
			}
		}
	}

	public void OnTurnComplete(LinkedList<Tile> tiles) {
		UserData uData = GameResources.Instance.GetUserData();
		QuestData qData = GameResources.Instance.GetQuestData();

		foreach(QuestProgressData pd in uData.GetActiveQuests(QuestType.Game, true)) {
			QuestItem qi = qData.GetById(pd.QuestId);
			if(qi.ConditionType == QuestConditionType.CollectСhain &&
			   qi.ChainLength <= tiles.Count) {
				GameResources.Instance.IncreasQuestAction(pd.QuestId, 1, false);
			}
		}
	}
		
	public void IncreaseActiveQuest(QuestType type, QuestConditionType cond, int count) {
		UserData uData = GameResources.Instance.GetUserData();
		QuestData qData = GameResources.Instance.GetQuestData();

		foreach(QuestProgressData pd in uData.GetActiveQuests(type, true)) {
			QuestItem qi = qData.GetById(pd.QuestId);
			if(qi.ConditionType == cond) {
				GameResources.Instance.IncreasQuestAction(pd.QuestId, count, false);
			}
		}
	}

	public void WinEnemy() {
		UserData uData = GameResources.Instance.GetUserData();
		if(uData.Level != App.CurrentLevel) {
			return;
		}
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.WinEnemy, 1);
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.WinEnemyInSequence, 1);

	}

	public void OnDeath() {
		UserData uData = GameResources.Instance.GetUserData();
		QuestData qData = GameResources.Instance.GetQuestData();

		foreach(QuestProgressData pd in uData.GetActiveQuests(QuestType.Game, true)) {
			QuestItem qi = qData.GetById(pd.QuestId);
			if(qi.ConditionType == QuestConditionType.WinEnemyInSequence) {
				GameResources.Instance.IncreasQuestAction(pd.QuestId, 0, false, true, false);
			}
		}
	}

	public void UseMagic() {
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.UseMagic, 1);
	}

	public void UseFortuna() {
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.UseFortuna, 1);
	}

	public void UseKachalka() {
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.UseKachalka, 1);
	}

	public void UseGoods() {
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.UseGoods, 1);
	}

	public void UseBlathata() {
		IncreaseActiveQuest(QuestType.Game, QuestConditionType.UseBlathata, 1);
	}

	public void SendGift(QuestType type, int count) {
		IncreaseActiveQuest(type, QuestConditionType.SendGift, count);
		FireQuestUpdate();
	}

	public void ReceiveGift(QuestType type, int count) {
		IncreaseActiveQuest(type, QuestConditionType.GetGift, count);
		FireQuestUpdate();
	}
}
