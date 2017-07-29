using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class LevelSettingsScene : WindowScene {
	public OnOffButton SoundButton;
	public OnOffButton MusicButton;
	public Text Description;
	public GameObject RepeatButton;

	private LocalData localData;

	void Start() {
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);

		localData = GameResources.Instance.GetLocalData();
		SoundButton.Toggle(localData.SoundOn);
		MusicButton.Toggle(localData.MusicOn);

		Description.text = App.CurrentLevel.ToString() + ": " + levelData.Name;

		RepeatButton.GetComponent<BuyButton>().Init(UserAssetType.Energy, levelData.LevelPrice, null);

	}

	public void OnCapitulate () {
		ParametersController.Instance.SetParameter(ParametersController.CAPITULATE_NOT_ENDED, true);
		SceneController.Instance.LoadSceneAsync(LevelFailureScene.SceneName);
	}


	public void OnSoundButton() {
		localData.SoundOn	= !localData.SoundOn;
		SoundButton.Toggle(localData.SoundOn);
		GameResources.Instance.SaveLocalData();
	}

	public void OnMusicButton() {
		localData.MusicOn	= !localData.MusicOn;
		MusicButton.Toggle(localData.MusicOn);
		GameResources.Instance.SaveLocalData();
	}
}
