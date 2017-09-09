using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Facebook.Unity;

public class LocationScene : BaseScene {
	public const string SceneName = "Location";

	public GameObjectResources GOResources;
	public UserAssetsPanel AssetPanel;
	public GameObject[] LocationMaps;
	public Sprite QuestionMark;
	public PanCamera mCamera;

	public GameObject GiftAttention;
	public GameObject QuestAttention;
	public GameObject FortunaAttention;
	public GameObject BlathataAttention;
	public GameObject KachalkaAttention;
	public GameObject GoodsAttention;

	private GameObject currentTouchObject;
	private GameObject currentLocationMap;
	private LocationData locationData;

	private Shader grayscale;

	public GameObject GiftButton;

	void OnEnable() {
		FBController.onLogin += OnSocialLogin;
		FBController.onLogout += OnSocialLogout;
		GameResources.Instance.onCompleteQuest += OnCompleteQuest;
		GameTimers.Instance.onTimerFortuna += OnTimerFortuna; 
		if(SceneControllerHelper.instance != null) {
			SceneControllerHelper.instance.onUnloadScene += OnUnloadScene;
		}
		GameResources.Instance.onCheckGift += OnCheckGift;
	}

	void OnDisable() {
		FBController.onLogin -= OnSocialLogin;
		FBController.onLogout -= OnSocialLogout;
		GameResources.Instance.onCompleteQuest -= OnCompleteQuest;
		GameTimers.Instance.onTimerFortuna -= OnTimerFortuna;
		if(SceneControllerHelper.instance != null) {
			SceneControllerHelper.instance.onUnloadScene -= OnUnloadScene;
		}
		GameResources.Instance.onCheckGift -= OnCheckGift;
	}

	void Start () {
		
		currentLocationMap = Instantiate(LocationMaps[App.CurrentLocation - 1], Vector3.zero, Quaternion.identity);

		locationData = GameResources.Instance.GetMapData().CityData[App.CurrentCity-1].LocationData[App.CurrentLocation-1];	

		foreach(Transform tr in currentLocationMap.transform) {
			tr.gameObject.SetActive(false);
		}

		int i = 1;
		int startLevel = GameResources.Instance.GetMapData().GetLevel(App.CurrentCity, App.CurrentLocation, 1);
		LocalData localData = GameResources.Instance.GetLocalData();
		UserData uData = GameResources.Instance.GetUserData();

		grayscale = Shader.Find("Custom/Greyscale");
		if(grayscale == null) {
			ModalPanels.Show(ModalPanelName.MessagePanel, "null");
		}

		bool locationContainLastLevel = startLevel <= localData.LastLevel && localData.LastLevel < startLevel + locationData.LevelsCountActual;
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
				Vector3 pos = levelGO.transform.position;
				pos.z = mCamera.transform.position.z;
				mCamera.SetPosition(pos);
			}

			i++;
		}

		UpdateGiftButton();
		UpdateFortunaAttention();
		UpdateQuestAttention();
		UpdateBlathataAttention();
		UpdateKachalkaAttention();
		UpdateGoodsAttention();

		Invoke("ShowAdditionScenes", 2);
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
		int level = GameResources.Instance.GetMapData().GetLevel(App.CurrentCity, App.CurrentLocation, locationLevel);
		UserData uData = GameResources.Instance.GetUserData();

		if(level > uData.Level) {
			return;
		}

		App.CurrentLocationLevel = locationLevel;

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

		DisableAdditiveScenes();
		SceneController.Instance.LoadSceneAdditive(LevelTargetScene.SceneName);
	}

	public void OnClickFB() {
		if(!Account.Instance.IsLogged) {
			SceneController.Instance.LoadSceneAdditive("FBNotLogged");
		} else {
			SceneController.Instance.LoadSceneAdditive("FBLogged");
		}
	}

	void UpdateGiftButton() {
		if(Account.Instance.IsLogged) {
			GiftButton.SetActive(true);
			GameObject attention = GiftButton.transform.Find("Attention").gameObject;
			attention.SetActive(GameResources.Instance.GetUserData().GetReceivedGiftUserIds().Length > 0);
		} else {
			GiftButton.SetActive(false);
		}
	}

	void OnSocialLogin() {
		UpdateGiftButton();
	}
	void OnSocialLogout() {
		UpdateGiftButton();
	}

	void UpdateFortunaAttention() {
		UserData uData = GameResources.Instance.GetUserData();
		FortunaAttention.SetActive(uData.FortunaTryCount > 0);
	}

	void UpdateQuestAttention() {
		UserData uData = GameResources.Instance.GetUserData();

		bool isAttention = uData.UpDateQuests(QuestType.Game);
		if(!isAttention) {
			IList<QuestProgressData> list = uData.GetActiveQuests(QuestType.Game, false);
			foreach(QuestProgressData item in list) {
				if(item.IsComplete) {
					isAttention = true;
					break;
				}
			}
		}

		QuestAttention.SetActive(isAttention);
	}

	void UpdateBlathataAttention() {
		UserData uData = GameResources.Instance.GetUserData();
		BlathataAttention.SetActive(uData.GetAsset(UserAssetType.Star).Value > 0);
	}

	void UpdateKachalkaAttention() {
		UserData uData = GameResources.Instance.GetUserData();
		KachalkaAttention.SetActive(uData.IsKachalkaAvaliable());
	}

	void UpdateGoodsAttention() {
		UserData uData = GameResources.Instance.GetUserData();
		GoodsAttention.SetActive(uData.IsGoodsAvaliable());
	}

	void OnCompleteQuest(QuestItem quest) {
		QuestAttention.SetActive(true);
	}

	public void OnClickQuestScene() {
		SceneController.Instance.LoadSceneAdditive(QuestScene.SceneName);
	}

	public void OnClickFortunaScene() {
		SceneController.Instance.LoadSceneAdditive(FortunaScene.SceneName);
	}

	void OnTimerFortuna(int timerCount) {
		if(timerCount == 0) {
			FortunaAttention.SetActive(true);
		}
	}

	void OnUnloadScene (string name, object retVal) {
		if(name == QuestScene.SceneName) {
			UpdateQuestAttention();
		} else if(name == FortunaScene.SceneName) {
			UpdateFortunaAttention();
		} else if(name == GiftScene.SceneName) {
			UpdateGiftButton();
		} else if(name == BlathataScene.SceneName) {
			UpdateBlathataAttention();
		} else if(name == KachalkaScene.SceneName) {
			UpdateKachalkaAttention();
		} else if(name == GoodsScene.SceneName) {
			UpdateGoodsAttention();
		}
	}

	public void OnClose() {
		//GameResources.Instance.SaveUserData(null, true);
		SceneController.Instance.LoadSceneAsync(CityScene.SceneName);
	}

	void OnCheckGift (string[] ids) {
		UpdateGiftButton();
	}

	public void OnClickGiftScene() {
		bool hasGift = GameResources.Instance.GetUserData().GetReceivedGiftUserIds().Length > 0;
		SceneController.Instance.LoadSceneAdditive(GiftScene.SceneName, (hasGift)? GiftScene.Type.Receive : GiftScene.Type.Send);
	}
}
