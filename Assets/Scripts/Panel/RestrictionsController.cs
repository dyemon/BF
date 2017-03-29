using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestrictionsController : MonoBehaviour {
	private LevelData levelData;
	private int currentMovesCount;
	private Text textMovesScore;

	public void LoadCurrentLevel () {
		levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		currentMovesCount = levelData.RestrictionData.Turns;

		textMovesScore = Preconditions.NotNull(transform.Find("Image").Find("Moves Score").gameObject.GetComponent<Text>(),"Can not get moves score");
		textMovesScore.text = currentMovesCount.ToString();
	}

	public void DecrementMoveScore() {
		currentMovesCount--;
		textMovesScore.text = currentMovesCount.ToString();
	}

	public bool CheckRestrictions() {
		return (currentMovesCount > 0);
	}
}
