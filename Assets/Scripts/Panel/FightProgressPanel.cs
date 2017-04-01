using UnityEngine;
using System.Collections;
using Common.Net.Http;
using UnityEngine.UI;

public class FightProgressPanel : MonoBehaviour, IResizeListener {
	public GameObject[] ResizedGO;

	public Text UserDamageText;
	public Text UserHealthText;
	public Text EnemyDamageText;
	public Text EnemyHealthText;

	public ProgressBar UserPs;
	public ProgressBar EnemyPs;

	public GameObject IndicatorFull;

	public Camera camera;

	private Rect userDamageRect;
	private Rect userHealthRect;
	private Rect enemyDamageRect;
	private Rect enemyHealthRect;
	private int screenHeight;
	private int screenWidth;

	private UserData userData;
	private GameData gameData;
	private LevelData levelData;

	private int currentUserHealth;


	public void OnResize(float resizeRation, Vector2 size) {
	//	transform.localScale = new Vector3(1, 1, 1);
	//	Vector2 bounds = UnityUtill.GetBounds(gameObject);
	//	float rRatio = size.x / bounds.x;
	//	transform.localScale = new Vector3(rRatio, 1, 1); 

		foreach(GameObject go in ResizedGO) {
			go.transform.localScale = new Vector3(1, 1, 1);
			Vector2 bounds = UnityUtill.GetBounds(go);
			float rRatio = size.x / bounds.x;
			go.transform.localScale = new Vector3(rRatio, 1, 1); 
		}

		UserPs.OnResize();
		EnemyPs.OnResize();
	}

	void Start() {
		levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		gameData = GameResources.Instance.GetGameData();
		userData = GameResources.Instance.GetUserData();
		currentUserHealth = userData.Health;

		if(camera == null) {
			camera = Camera.main;
		}

		UserDamageText.text = userData.Damage.ToString();
		UserHealthText.text = userData.Health.ToString();

		UserPs.SetMaxValue(GameData.PowerPointSuccess);
		EnemyPs.SetMaxValue(GameData.EnemyTurn);

		ShowFullIndicator(false);
	}

	private void ShowFullIndicator(bool show) {
		if(IndicatorFull != null) {
			IndicatorFull.GetComponent<SpriteRenderer>().enabled = show;
		}
	}
	/*
	private void CalcUIRects() {
		Vector3 screenPos = camera.WorldToScreenPoint(UserDamagePos.transform.position);
		userDamageRect = new Rect(screenPos.x, Screen.height - screenPos.y, 70, 50);

		Debug.Log(UserDamagePos.transform.position);
		Debug.Log(screenPos);
	}*/

	public bool UpdateUserEvaluatePowerPoints(float points) {
		bool res = UserPs.SetEvaluteProgress((int)Mathf.Round(points));
		ShowFullIndicator(res);
		return res;
	}

	public bool IncreesUserProgress(float points) {
		bool res = UserPs.IncreesProgress((int)Mathf.Round(points), true);
		ShowFullIndicator(res);
		return res;
	}
}