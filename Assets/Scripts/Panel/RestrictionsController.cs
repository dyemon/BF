using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestrictionsController : MonoBehaviour {
	private LevelData levelData;
	private int currentTurnsCount;
	private Text textTurnsScore;
	private int turnsCount;

	public void LoadCurrentLevel () {
		levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		turnsCount = currentTurnsCount = levelData.RestrictionData.Turns;

		if(turnsCount > 0) {
			textTurnsScore = Preconditions.NotNull(transform.Find("Image").Find("Moves Score").gameObject.GetComponent<Text>(), "Can not get moves score");
			textTurnsScore.text = turnsCount.ToString();
		} else {
			gameObject.SetActive(false);
		}
	}

	public void DecrementMoveScore() {
		if(turnsCount == 0) {
			return;
		}

		currentTurnsCount--;
		textTurnsScore.text = currentTurnsCount.ToString();
	}

	public bool CheckRestrictions() {
		return (currentTurnsCount > 0 || turnsCount == 0);
	}

	public void IncreaseCurrentTurns(int turns) {
		currentTurnsCount += turns;
		textTurnsScore.text = currentTurnsCount.ToString();
	}
}
