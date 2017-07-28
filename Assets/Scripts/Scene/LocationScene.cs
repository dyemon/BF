using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LocationScene : BaseScene {
	public const string SceneName = "Location";

	public GameObjectResources GOResources;
	public UserAssetsPanel AssetPanel;
	public GameObject[] LocationMaps;
	public Sprite QuestionMark;
	public PanCamera mCamera;

	private GameObject currentTouchObject;
	private GameObject currentLocationMap;
	private LocationData locationData;

	private Shader grayscale;

	void Start () {

		currentLocationMap = Instantiate(LocationMaps[App.CurrentLocation - 1], Vector3.zero, Quaternion.identity);

		locationData = GameResources.Instance.GetMapData().CityData[App.CurrentCity-1].LocationData[App.CurrentLocation-1];	

		foreach(Transform tr in currentLocationMap.transform) {
			tr.gameObject.SetActive(false);
		}

		int i = 1;
		int startLevel = GameResources.Instance.GetMapData().GetLevel(App.CurrentCity, App.CurrentLocation, 1);
		LocalSettingsData localData = GameResources.Instance.GetLocalSettings();
		UserData uData = GameResources.Instance.GetUserData();

		grayscale = Shader.Find("Custom/Greyscale");

		bool locationContainLastLevel = startLevel <= localData.LastLevel && localData.LastLevel < startLevel + locationData.LevelsCount;
		foreach(LocationLevelData levelData in locationData.LevelData) {
			GameObject levelGO = currentLocationMap.transform.Find("Level" + i).gameObject;
			levelGO.SetActive(true);
			int curLevel = startLevel + i - 1;

			SpriteRenderer sr = levelGO.transform.Find("Button").GetComponent<SpriteRenderer>();
			sr.sprite = GOResources.GetCheckpoinButton(curLevel, uData.Level, localData.LastLevel);
	//		sr.sprite = GOResources.GetCheckpoinButton(curLevel, 3,2);
	//		uData.Level = 3;
			sr = levelGO.transform.Find("Icon").GetComponent<SpriteRenderer>();
			Sprite icon = null;
			if(levelData.EnemyType != null && (!levelData.Hidden || curLevel <= uData.Level)) {
				icon = GOResources.GetEnemyIcon(levelData.EnemyType.Value);
			} else if(levelData.Hidden && curLevel > uData.Level) {
				icon = QuestionMark;
			}
			sr.sprite = icon;

			if(sr.sprite != null && curLevel > uData.Level) {
				sr.material.shader = grayscale;
			}

			TextMeshPro levelText = levelGO.transform.Find("LevelText").GetComponent<TextMeshPro>();
			levelText.text = curLevel.ToString();

			if((locationContainLastLevel && curLevel == localData.LastLevel) ||
				(!locationContainLastLevel && curLevel == uData.Level)) {
		//		mCamera.SetPosition(levelGO.transform.position);
			}

			i++;
		}
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
