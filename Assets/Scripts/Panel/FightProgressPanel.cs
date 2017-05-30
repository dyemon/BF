using UnityEngine;
using System.Collections;
using Common.Net.Http;
using UnityEngine.UI;

public class FightProgressPanel : MonoBehaviour, IResizeListener {
	public GameObject[] ResizedGO;

	public Text HeroDamageText;
	public Text HeroHealthText;
	public Text EnemyDamageText;
	public Text EnemyHealthText;

	public ProgressBar HeroPs;
	public ProgressBar EnemyPs;

	public GameObject IndicatorFull;

	private UserData userData;
	private GameData gameData;
	private LevelData levelData;

	private int currentUserHealth;
	private int currentEnemyTurns;

	private HeroController heroController;
	private EnemyController enemyController;

	public void OnResize(float resizeRation, Vector2 size) {
		/*
		foreach(GameObject go in ResizedGO) {
			go.transform.localScale = new Vector3(1, 1, 1);
			Vector2 bounds = UnityUtill.GetBounds(go);
			float rRatio = size.x / bounds.x;
			go.transform.localScale = new Vector3(rRatio, 1, 1); 
		}

		HeroPs.OnResize();
		EnemyPs.OnResize();
		*/
	}

	void Start() {
		levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		gameData = GameResources.Instance.GetGameData();
		userData = GameResources.Instance.GetUserData();
		currentUserHealth = userData.Health;
		currentEnemyTurns = 0;



		ShowFullIndicator(false);
	}

	public void Init(HeroController hc, EnemyController ec) {
		heroController = hc;
		enemyController = ec;

		HeroDamageText.text = (enemyController != null)? heroController.Damage.ToString() : "";
		HeroHealthText.text = (enemyController != null)? heroController.Health.ToString() : "";
		EnemyDamageText.text = (enemyController != null)? enemyController.Damage.ToString() : "";
		EnemyHealthText.text = (enemyController != null)? enemyController.Health.ToString() : "";

		HeroPs.SetMaxValue(heroController.PowerPointSuccess);
		EnemyPs.SetMaxValue((enemyController != null)? enemyController.TurnsSuccess : 0);
	}

	private void ShowFullIndicator(bool show) {
		if(IndicatorFull != null) {
			IndicatorFull.GetComponent<SpriteRenderer>().enabled = show;
		}
	}

	public bool UpdateHeroEvaluatePowerPoints(float points) {
		bool res = HeroPs.SetEvaluteProgress((int)Mathf.Round(points));
		ShowFullIndicator(res);
		return res;
	}

	public void UpdateProgress() {
		if(heroController == null) {
			return;
		}

		EnemyPs.SetProgress((enemyController != null)? enemyController.CurrentTurns : 0, true);
		HeroPs.SetProgress(heroController.CurrentPowerPoints, true);
		ShowFullIndicator(heroController.IsStrike);
	}

	public void UpdateFightParams() {
		HeroDamageText.text = heroController.Damage.ToString();
		HeroHealthText.text = heroController.Health.ToString();
		EnemyDamageText.text = enemyController.Damage.ToString();
		EnemyHealthText.text = enemyController.Health.ToString();
	}

	public void KillEnemy() {
		EnemyPs.SetProgress(0, false);
		EnemyPs.ShowText(false);
		EnemyDamageText.enabled = false;
		EnemyHealthText.enabled = false;
	}
}