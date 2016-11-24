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
		levelData = GameResources.Instance.LoadLevel(App.GetCurrentLevel());
		success = new bool[levelData.TargetData.Length];

		foreach(TargetType type in Enum.GetValues(typeof(TargetType))) {
			if(TileItem.IsColorItem((TileItemType)type)) {
				targetIcons.Add(type, Icons[((int)type)/TileItem.TILE_ITEM_GROUP_WEIGHT]);
			} else if(TileItem.IsSpecialCollectItem((TileItemType)type)) {
				targetIcons.Add(type, Icons[5 + (int)type - (int)TileItemTypeGroup.Special]);
			} 
		}

		targetIcons.Add(TargetType.BombAll, Icons[7]);
		targetIcons.Add(TargetType.Box, Icons[8]);

		int index = 0;
		foreach(TargetData data in levelData.TargetData) {
			GameObject target = Instantiate(TargetGO);
			target.transform.SetParent(gameObject.transform);
			target.transform.localScale = new Vector3(1, 1, 1);
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
		int index = 0;

		if(!levelLoaded) {
			return;
		}

		bool checkByTypeGroup = tileItem.IsColor || tileItem.IsBox;
		foreach(TargetData data in levelData.TargetData) {
			if(((int)data.Type == (int)tileItem.Type) && !checkByTypeGroup || ((int)data.Type == (int)tileItem.TypeGroup) && checkByTypeGroup) {
				GameObject targetGO = Preconditions.NotNull(transform.GetChild(index).gameObject, "Can not get target game object for index {0}", index);
				Text text = targetGO.transform.Find("Text").gameObject.GetComponent<Text>();
				if(text == null || success[index]) {
					break;
				}
				int count = Int32.Parse(text.text);

				if(count == 0) {
					break;
				}
				if(--count == 0) {					
					Image sImage = Instantiate(successImage);
					sImage.transform.SetParent(targetGO.transform);
					sImage.transform.localScale = new Vector3(1, 1, 1);
					sImage.transform.position = new Vector3(text.transform.position.x, text.transform.position.y, text.transform.position.z);
					Destroy(text);
					success[index] = true;;
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
