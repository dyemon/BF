using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public delegate void OnStrik();

	public static float STRIKE_DELAY = 1;

	EnemyData enemyData;

	public int Health { get; set; }
	public int CurrentTurns { get; set; }

	public int Damage {
		get { 
			return enemyData.Damage; 
		}
	}
		
	public int TurnsSuccess {
		get { 
			return GameData.EnemyTurns; 
		}
	}

	public bool IsStrik {
		get { return CurrentTurns >= TurnsSuccess; }
	}

	void Start () {
		enemyData = GameResources.Instance.GetLevel(App.GetCurrentLevel()).EnemyData;
		if(enemyData == null) {
			return;
		}

		Health = enemyData.Health;
		ResetTurns();
	}

	public void IncreesTurns(int turns) {
		CurrentTurns += turns;
	}
		
	public void ResetTurns() {
		CurrentTurns = 0;
	}

	public void Strike(OnStrik onStrik) {
		DisplayMessageController.DisplayMessage("Удар врага", Color.red);
		StartCoroutine(StrikInternal(onStrik));

	}

	private IEnumerator StrikInternal(OnStrik onStrik) {
		yield return new WaitForSeconds(STRIKE_DELAY);
		ResetTurns();
		onStrik();
	}

	public void DecreesHealt(int damage) {
		Health -= damage;
		if(Health < 0) {
			Health = 0;
		}
	}

	public bool IsDeath(int evaluteValue) {
		return Health - evaluteValue <= 0;
	}
}
