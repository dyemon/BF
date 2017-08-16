using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TargetController : MonoBehaviour {
	private LevelData levelData;
	public GameObjectResources GameObjectResources;

	public GameObject TargetGO;
	public Image successImage;
	private bool[] success;
	private bool levelLoaded = false;

	public void LoadCurrentLevel () {
		levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		success = new bool[levelData.TargetData.Length];


		int index = 0;
		foreach(TargetData data in levelData.TargetData) {
			GameObject target = Instantiate(TargetGO);
			target.transform.SetParent(gameObject.transform);
			target.transform.localScale = new Vector3(1, 1, 1);
			Image icon = target.transform.Find("Image").gameObject.GetComponent<Image>();
			icon.sprite = GameObjectResources.GetTargetIcon(data.Type);
			Text text = target.transform.Find("Text").gameObject.GetComponent<Text>();
			text.text = data.Count.ToString();
			success[index++] = (data.Count > 0)? false : true;
		}

		levelLoaded = true;
	}

	public static bool EqualsByType(TileItem tileItem, TargetType tType) {
		bool checkByTypeGroup = tileItem.IsColor || tileItem.IsBox;
		if(((int)tType == (int)tileItem.Type) && !checkByTypeGroup || ((int)tType == (int)tileItem.TypeGroup) && checkByTypeGroup) {
			return true;
		}
		return false;
	}
		
	public void OnCollectTileItem(TileItem tileItem) {
		Preconditions.NotNull(tileItem, "Collected tile item can not be null");
		int index = 0;

		if(!levelLoaded) {
			return;
		}


		foreach(TargetData data in levelData.TargetData) {
			if(EqualsByType(tileItem, data.Type)) {
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
					SetSuccess(targetGO, index);
				} else {
					text.text = count.ToString();
				}
				break;
			}
			index++;
		}
	}

	private void SetSuccess(GameObject targetGO, int index) {
		Image sImage = Instantiate(successImage);
		sImage.transform.SetParent(targetGO.transform);
		sImage.transform.localScale = new Vector3(1, 1, 1);
		Text text = targetGO.transform.Find("Text").gameObject.GetComponent<Text>();
		sImage.transform.position = new Vector3(text.transform.position.x, text.transform.position.y, text.transform.position.z);
		Destroy(text);
		success[index] = true;
	}

	public void ClearPanel() {
		for(int i = 1; i < gameObject.transform.childCount; i++) {
			Destroy(gameObject.transform.GetChild(i).gameObject);
		}
	}

	public void UnloadLevel() {
		//ClearPanel();
	//	levelLoaded = false;
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

	public void KillEnemy() {
		int index = 0;
		foreach(TargetData data in levelData.TargetData) {
			if(data.Type == TargetType.Enemy) {
				GameObject targetGO = Preconditions.NotNull(transform.GetChild(index).gameObject, "Can not get target game object for index {0}", index);
				SetSuccess(targetGO, index);
			}
			index++;
		}
	}

	public IList<TileItemTypeGroup> GetColorNecessaryGroup() {
		IList<TileItemTypeGroup> res = new List<TileItemTypeGroup>();
		int index = -1;

		foreach(TargetData data in levelData.TargetData) {
			index++;
			if(data.Type == TargetType.Enemy) {
				continue;
			}
			TileItemType tiType = (TileItemType)data.Type; 
			if(!TileItem.IsColorItem(tiType)) {
				continue;
			}

			GameObject targetGO = Preconditions.NotNull(transform.GetChild(index).gameObject, "Can not get target game object for index {0}", index);
			Text text = targetGO.transform.Find("Text").gameObject.GetComponent<Text>();
			if(text == null || success[index]) {
				continue;
			}
			int count = Int32.Parse(text.text);
			if(count > 0) {
				res.Add(TileItem.TypeToTypeGroup(tiType));
			}
		}

		return res;
	}
}
