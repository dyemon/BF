using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour {
	public delegate void OnStrik();

	UserData userData;
	public int Health { get; set; }
	public int CurrentPowerPoints { get; set; }

	public int Damage {
		get { 
			return userData.Damage; 
		}
	}

	public int PowerPointSuccess {
		get { 
			return GameData.PowerPointSuccess; 
		}
	}

	public bool IsStrik {
		get { return CurrentPowerPoints >= PowerPointSuccess; }
	}

	void Start () {
		userData = GameResources.Instance.GetUserData();
		Health = userData.Health;
		ResetPowerPoints();
	}
		
	public void IncreesPowerPoints(int points) {
		CurrentPowerPoints += points;
	}



	public void ResetPowerPoints() {
		CurrentPowerPoints = 0;
	}

	public void Strike(OnStrik onStrik) {
		DisplayMessageController.DisplayMessage("Удар героя", Color.green);
		ResetPowerPoints();
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
