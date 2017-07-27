using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocationScene : BaseScene {
	public const string SceneName = "Location";

	public UserAssetsPanel AssetPanel;

	private GameObject currentTouchObject;

	void Start () {
		
	}
	
	void Update() {
		InputController.Touch[] touches = InputController.getTouches();

		if(touches.Length > 0 && (touches[0].phase == TouchPhase.Began || touches[0].phase == TouchPhase.Ended)) {
			if(EventSystem.current.IsPointerOverGameObject(InputController.GetFingerId())) {
				return;
			}

			Ray ray = InputController.TouchToRay(touches[0]);

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
			if(hit.collider != null) {		
				if(touches[0].phase == TouchPhase.Began) {
					currentTouchObject = hit.collider.gameObject; 
				} else if(hit.collider.gameObject == currentTouchObject) {
					OnSelectGameObject(hit.collider.gameObject);
				}
			}
		}
	}

	void OnSelectGameObject(GameObject go) {
		string name = go.transform.parent.name;

		if(string.IsNullOrEmpty(name)) {
			return;
		}

		if(name.StartsWith("Level")) {
			OnSelectLevel(go.transform.parent.gameObject, System.Int32.Parse(name.Replace("Level", "")));
		}
			
	}

	void OnSelectLevel(GameObject go, int locationLevel) {
		App.CurrentLocationLevel = locationLevel;
		int level = GameResources.Instance.GetMapData().GetLevel(App.CurrentCity, App.CurrentLocation, App.CurrentLocationLevel);

		if(level < 0) {
			DisplayMessageController.ShowUnavaliableLevelMessage();
			return;
		}

		App.CurrentLevel = level;
		Debug.Log(locationLevel + " " + App.CurrentLevel);
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);
		if(levelData == null) {
			DisplayMessageController.ShowUnavaliableLevelMessage();
			return;
		}
	}
}
