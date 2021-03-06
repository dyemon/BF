﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Timer;
using Common.Animation;
using UnityEngine.Advertisements;

public class FortunaScene : WindowScene {
	public const string SceneName = "Fortuna";

	public GameObject Ruletka;

	private bool rotateRuletka;
	private int angle;

	public GameObjectResources GOResources;
	public Button TimerButton;
	public GameObject PrizeItem;
	public UserAssetsPanel AssetsPanel;
	public GameObject pointer;

	private FortunaPrizeItem[] prizeItems = new FortunaPrizeItem[10];
	private FortunaData fData;

	private const int deg = 36;

	void OnEnable() {
		GameTimers.Instance.onTimerFortuna += OnTimerFortuna; 
	}

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
		GameTimers.Instance.onTimerFortuna -= OnTimerFortuna;
	}

	protected override void Start() {
		base.Start();

		fData = GameResources.Instance.GetGameData().FortunaData;
		UserData uData = GameResources.Instance.GetUserData();
		UpdateTimerButton(uData.FortunaTryCount, GetTimerCount());

		if(uData.FortunaTryCount > 0) {
			ParametersController.Instance.SetParameter(ParametersController.FORTUNA_IS_SHOWN, true);
		}

		prizeItems[0] = fData.GetItem(UserAssetType.Ring);
		prizeItems[1] = fData.GetItem(UserAssetType.Mobile);
		prizeItems[2] = fData.GetItem(UserAssetType.Energy);
		prizeItems[3] = fData.GetItem(UserAssetType.Ring);
		prizeItems[4] = fData.GetItem(UserAssetType.Mobile);
		prizeItems[5] = fData.GetItem(UserAssetType.Energy);
		prizeItems[6] = fData.GetItem(UserAssetType.Ring);
		prizeItems[7] = fData.GetItem(UserAssetType.Mobile);
		prizeItems[8] = null;
		prizeItems[9] = fData.GetItem(UserAssetType.Energy);

		App.InitAds();
	}

	int GetTimerCount() {
		if(TimerController.Instance == null) {
			return 0;
		}
		Timer timer = TimerController.Instance.GetTimer(GameTimers.FORTUNA_TIMER_CODE);
		return (timer == null) ? 0 : timer.GetCurCount();
	}

	public void OnStartClick() {
		if(Advertisement.IsReady()) {
			Advertisement.Show(new ShowOptions() { resultCallback = AdsCallback });
		} else {
			ModalPanels.Show(ModalPanelName.ErrorPanel, "Сервис не доступен. Проверте соединение с сетью", () => {App.InitAds(); });
		}
	}

	void AdsCallback(ShowResult res) {
		if(res == ShowResult.Finished) {
			OnRuletkaStart();
		}
	}

	public void OnRuletkaStart() {
		Vector3 rotate = Vector3.zero;
		rotate.z = Random.Range(1440, 1440 + 360);

		float az = Ruletka.transform.eulerAngles.z - deg / 2;
		if(az < 0) {
			az = 360 + az;
		}
		float curAngle = Mathf.Round(az)%deg;
		 
		float prevAngle = Ruletka.transform.eulerAngles.z;
		Debug.Log("Start " + az + " " + curAngle + " " + prevAngle);
		AnimatedObject ao = Ruletka.GetComponent<AnimatedObject>();
		ao.AddRotate(null, rotate, 5, new ABase.BezierPoints(0.8f, 0.9f, 0.9f, 0.999f))
			.OnStop(() => {
				OnRuletkaStop();
			})
			.OnStep((t, go) => {
				float zr = go.transform.eulerAngles.z;
				if(prevAngle > zr) {
					zr += 360;
				}
				curAngle += (zr - prevAngle);

				if(curAngle >= deg) {
					Debug.Log(zr + " " + prevAngle + " " + curAngle);
					SoundController.Play(SoundController.Instance.FortunaRotate);
					curAngle = curAngle - deg;
				}
				prevAngle = go.transform.eulerAngles.z;;
			})
			.Build().Run();	

		TimerButton.interactable = false;
	}

	void Update() {
	/*	if(!rotateRuletka) {
			return;
		}

		int speed = 1;
		int rotateAngle = Time.deltaTime * speed;
		angle -= rotateAngle;
		*/
	}

	void OnRuletkaStop() {
		
		float angle = Ruletka.transform.rotation.eulerAngles.z + deg/2;
	//	Debug.Log(angle);
		if(angle >= 360) {
			angle = angle - 360;
		}

		int index = (int)Mathf.Floor(angle / deg);
	//	Debug.Log(angle + " " + index);
		FortunaPrizeItem pItem = prizeItems[index] == null? fData.GetJackpotItem() : prizeItems[index];
		if(pItem.IsUserAssetPrize()) {
			AwardUserAsset(pItem.UserAssetType, pItem.GetPrizeAmount());

			if(prizeItems[index] == null) {
				SoundController.Play(SoundController.Instance.FortunaJackpot);
			}
			SoundController.Play(SoundController.Instance.Coins, SoundController.COINS_VOLUME);
		}

		int tryCount = GameResources.Instance.DecreaseFortunaTryCount(1);
		UpdateTimerButton(tryCount, GetTimerCount());

		if(tryCount == 0) {
			ParametersController.Instance.SetParameter(ParametersController.FORTUNA_IS_SHOWN, false);
		}

		QuestController.Instance.UseFortuna();
	}

	void OnTimerFortuna(int timerCount) {
		int tryCount = 0;

		if(timerCount == 0) {
			tryCount = GameResources.Instance.GetUserData().FortunaTryCount;
		}
		UpdateTimerButton(tryCount, timerCount);
	}

	void UpdateTimerButton(int tryCount, int timerCount) {
		Text timerText = TimerButton.transform.Find("Text").GetComponent<Text>();

		if(tryCount > 0) {
			TimerButton.interactable = true;
			timerText.text = "Старт";
		} else {
			TimerButton.interactable = false;
			timerText.text = DateTimeUtill.FormatSeconds(timerCount);;
		}
	}

	void AwardUserAsset(UserAssetType type, int amount) {
		AssetsPanel.DisableUpdate(true);
		GameResources.Instance.ChangeUserAsset(type, amount);	
		AssetsPanel.DisableUpdate(false);


		Vector3 end = AssetsPanel.GetUserAssetsIcon(type).transform.position;
		Vector3 start = pointer.transform.position + new Vector3(0, 0, 0);
		GameObject animImg = Instantiate(PrizeItem, start, Quaternion.identity);
		animImg.transform.SetParent(transform);

		animImg.AddComponent<AnimatedObject>();
		Sprite icon = GOResources.GetUserAssetIcone(type);

		Animations.CreateAwardAnimation(animImg, start, end, icon, amount); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteAward(animImg);} ).Run();
	}

	void CompleteAward(GameObject animGO) {
		Destroy(animGO);
		AssetsPanel.UpdateUserAssets();
	}
}
