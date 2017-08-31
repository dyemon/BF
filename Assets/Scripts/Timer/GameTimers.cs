using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Timer;

public class GameTimers {
	public static readonly GameTimers Instance = new GameTimers();

	public const string ENERGY_TIMER_CODE = "ENERGY_TIMER_CODE";
	public const string INFINITY_ENERGY_TIMER_CODE = "INFINITY_ENERGY_TIMER_CODE";
	public const string FORTUNA_TIMER_CODE = "FORTUNA_TIMER_CODE";
	public const string CLEARFRENDSCACHE_TIMER_CODE = "CLEARFRENDSCACHE_TIMER_CODE";
	public const string GIFTCACHE_TIMER_CODE = "GIFTCACHE_TIMER_CODE";

	public delegate void OnTimerFortuna(int timerCount);
	public event OnTimerFortuna onTimerFortuna;

	public delegate void OnClearGiftCache();
	public event OnClearGiftCache onClearGiftCache;

	bool init = false;

	public void Init(UserData uData) {
		if(TimerController.Instance == null) {
			return;
		}

		Preconditions.Check(!init, "Timers Initialized again");

		GameData gData = GameResources.Instance.GetGameData();

		TimerController.Instance.AddTimer(ENERGY_TIMER_CODE)
			.SetPeriod(gData.EnergyData.IncreaseTime * 60).Start();

		if(uData.InfinityEnergyDuration > 0) {
			StartInfinityEnergyTimer();
		}

		if(uData.FortunaTryCount == 0) {
			long delta = uData.GetCurrentTimestamp() - uData.FortunaLastTry;
			int remainTime = (int)( gData.FortunaData.Delay * 60 - delta);
			if(remainTime <= 0) {
				uData.ResetFortunaTryCount();
			} else {
				StartFortunaTimer((uint)remainTime);
			}
		}

		TimerController.Instance.onTimer += OnTimer;
		init = true;
	}
		

	public void StartInfinityEnergyTimer() {
		Timer t = TimerController.Instance.GetTimer(INFINITY_ENERGY_TIMER_CODE);
		if(t == null) {
			t = TimerController.Instance.AddTimer(INFINITY_ENERGY_TIMER_CODE).SetPeriod(60);
		}
		t.Start();
	
	}

	public void StartClearFrendsCache() {
		Timer t = TimerController.Instance.GetTimer(CLEARFRENDSCACHE_TIMER_CODE);
		if(t == null) {
			t = TimerController.Instance.AddTimer(CLEARFRENDSCACHE_TIMER_CODE).SetPeriod(60 * 10)
				.SetCount(1);
		}
		t.Start();
	}

	public void StarGiftCach() {
		Timer t = TimerController.Instance.GetTimer(GIFTCACHE_TIMER_CODE);
		if(t == null) {
			t = TimerController.Instance.AddTimer(GIFTCACHE_TIMER_CODE).SetPeriod(60 * 10)
				.SetCount(1);
		}
		t.Start();
	}

	public void StartFortunaTimer(uint timerCount) {
		Timer t = TimerController.Instance.GetTimer(FORTUNA_TIMER_CODE);
		if(t == null) {
			GameData gData = GameResources.Instance.GetGameData();
			t = TimerController.Instance.AddTimer(FORTUNA_TIMER_CODE).SetPeriod(1)
				.SetCount((timerCount == 0)? gData.FortunaData.Delay * 60 : timerCount);
		}
		t.Start();
	}

	public void StopInfinityEnergyTimer() {
		TimerController.Instance.RemoveTimer(INFINITY_ENERGY_TIMER_CODE);
	}

	void OnTimer(string code, Timer timer) {
		GameData gData = GameResources.Instance.GetGameData();

		switch(code) {
		case ENERGY_TIMER_CODE:
			int eCount = GameResources.Instance.GetUserData().GetAsset(UserAssetType.Energy).Value;
			Debug.Log("Timer " + ENERGY_TIMER_CODE + " " );

			if(eCount < gData.EnergyData.MaxIncreaseCount) {
				GameResources.Instance.ChangeUserAsset(UserAssetType.Energy, 1);
			}
			break;
		case INFINITY_ENERGY_TIMER_CODE:
			bool live = GameResources.Instance.DecreaseInfinityEnergy(1);
			UserData uData = GameResources.Instance.GetUserData();
		//	Debug.Log("----------- " + INFINITY_ENERGY_TIMER_CODE + uData.InfinityEnergyDuration);
			if(!live) {
				StopInfinityEnergyTimer();
			}
			break;
		case FORTUNA_TIMER_CODE:
			int timerCount = timer.GetCurCount();
		//	Debug.Log(FORTUNA_TIMER_CODE + " " + timerCount);
			if(timerCount == 0) {
				GameResources.Instance.ResetFortunaTryCount();
			}
			if(onTimerFortuna != null) {
				onTimerFortuna(timerCount);
			}
			break;
		case CLEARFRENDSCACHE_TIMER_CODE:
		//	Debug.Log("----------- Clear friends cache");
			FBController.ClearFriendsCache();
			break;
		case GIFTCACHE_TIMER_CODE:
			Debug.Log("Timer ----------- GIFTCACHE_TIMER_CODE");
			ParametersController.Instance.SetParameter(ParametersController.RECEIVED_GIFT_CACHE_UPDATED, false);
			if(onClearGiftCache != null) {
				onClearGiftCache();
			}
			break;
		
		}

	}

	public void Stop() {
		TimerController.Instance.StopAll();
		FBController.ClearFriendsCache();
		ParametersController.Instance.SetParameter(ParametersController.RECEIVED_GIFT_CACHE_UPDATED, false);

		TimerController.Instance.onTimer -= OnTimer;
		init = false;
	}
}
