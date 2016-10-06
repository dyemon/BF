using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TargetController : MonoBehaviour {
	private LevelData levelData;

	private Dictionary<TargetType, Sprite> targetIcons = new Dictionary<TargetType, Sprite>();
	public Sprite[] Icons;
	public GameObject TargetGO;
	public Image successImage;
	private bool[] success;
	private bool levelLoaded = false;

	public void LoadCurrentLevel () {
		levelData = GameResources.LoadLevel(App.GetCurrentLevel());
		success = new bool[levelData.TargetData.Length];

		foreach(TargetType type in Enum.GetValues(typeof(TargetType))) {
			if(Icons.Length > ((int)type)/20) {
				targetIcons.Add(type, Icons[((int)type)/20]);
			}
		}

		int index = 0;
		foreach(TargetData data in levelData.TargetData) {
			GameObject target = Instantiate(TargetGO);
			target.transform.SetParent(gameObject.transform);
			Image icone = target.transform.Find("Image").gameObject.GetComponent<Image>();
			icone.sprite = targetIcons[data.Type];
			Text text = target.transform.Find("Text").gameObject.GetComponent<Text>();
			text.text = data.Count.ToString();
			success[index++] = (data.Count > 0)? false : true;
		}

		levelLoaded = true;
	}
	
	public void OnCollectTileItem(TileItem tileItem) {
		Preconditions.NotNull(tileItem, "Collected tile item can not be null");
		int index = 1;

		if(!levelLoaded) {
			return;
		}

		foreach(TargetData data in levelData.TargetData) {
			if((int)data.Type == (int)tileItem.Type) {
				GameObject targetGO = Preconditions.NotNull(transform.GetChild(index).gameObject, "Can not get target game object for index {0}", index);
				Text text = targetGO.transform.Find("Text").gameObject.GetComponent<Text>();
				if(text == null || success[index - 1]) {
					break;
				}
				int count = Int32.Parse(text.text);

				if(count == 0) {
					break;
				}
				if(--count == 0) {					
					Image sImage = Instantiate(successImage);
					sImage.transform.SetParent(targetGO.transform);
					sImage.transform.position = new Vector3(text.transform.position.x - 2, text.transform.position.y, text.transform.position.z);
					Destroy(text);
					success[index - 1] = true;;
				} else {
					text.text = count.ToString();
				}
				break;
			}
			index++;
		}
	}

	public void ClearPanel() {
		for(int i = 1; i < gameObject.transform.childCount; i++) {
			Destroy(gameObject.transform.GetChild(i).gameObject);
		}
	}

	public void UnloadLevel() {
		ClearPanel();
		levelLoaded = false;
	}

	public bool CheckSuccess() {
		foreach(bool item in success) {
			if(!item) {
				return false;
			}
		}
		return true;
	}

	public void LevelSuccess() {
		UnloadLevel();
	}

	public void LevelFailure() {
		UnloadLevel();
	}
}
