using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Timer;

public class EnergyTimers {
	public static readonly EnergyTimers Instance = new EnergyTimers();

	public const string ENERGY_TIMER_CODE = "ENERGY_TIMER_CODE";
	public const string INFINITY_ENERGY_TIMER_CODE = "INFINITY_ENERGY_TIMER_CODE";

	bool init = false;

	public void Init(UserData uData) {
		if(TimerController.Instance == null) {
			return;
		}

		Preconditions.Check(!init, "EnergyTimers Initialized again");

		GameData gData = GameResources.Instance.GetGameData();

		TimerController.Instance.AddTimer(ENERGY_TIMER_CODE)
			.SetPeriod(gData.EnergyData.IncreaseTime * 60).Start();

		if(uData.InfinityEnergyDuration > 0) {
			StartInfinityTimer();
		}

		TimerController.Instance.onTimer += OnTimer;
		init = true;
	}

	public void StartInfinityTimer() {
		Timer t = TimerController.Instance.GetTimer(INFINITY_ENERGY_TIMER_CODE);
		if(t == null) {
			t = TimerController.Instance.AddTimer(INFINITY_ENERGY_TIMER_CODE).SetPeriod(60);
		}
		t.Start();
	}

	public void StopInfinityTimer() {
		TimerController.Instance.RemoveTimer(INFINITY_ENERGY_TIMER_CODE);
	}

	void OnTimer(string code) {
		GameData gData = GameResources.Instance.GetGameData();

		switch(code) {
		case ENERGY_TIMER_CODE:
			int eCount = GameResources.Instance.GetUserData().GetAsset(UserAssetType.Energy).Value;
			if(eCount < gData.EnergyData.MaxIncreaseCount) {
				GameResources.Instance.ChangeUserAsset(UserAssetType.Energy, 1);
			}
			break;
		case INFINITY_ENERGY_TIMER_CODE:
			bool live = GameResources.Instance.DecreaseInfinityEnergy(1);
			if(!live) {
				StopInfinityTimer();
			}
			break;
		}
	}
}
