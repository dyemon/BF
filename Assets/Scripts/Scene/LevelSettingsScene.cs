using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class LevelSettingsScene : WindowScene {
	public OnOffButton SoundButton;
	public OnOffButton MusicButton;
	public Text Description;
	public GameObject RepeatButton;

	private LocalSettingsData localSettings;

	void Start() {
		LevelData levelData = GameResources.Instance.GetLevel(App.CurrentLevel);

		localSettings = GameResources.Instance.GetLocalSettings();
		SoundButton.Toggle(localSettings.SoundOn);
		MusicButton.Toggle(localSettings.MusicOn);

		Description.text = App.CurrentLevel.ToString() + ": " + levelData.Name;

		RepeatButton.GetComponent<BuyButton>().Init(UserAssetType.Energy, levelData.LevelPrice, null);

	}

	public void OnCapitulate () {
		ParametersController.Instance.SetParameter(ParametersController.CAPITULATE_NOT_ENDED, true);
		SceneController.Instance.LoadSceneAsync(LevelFailureScene.SceneName);
	}


	public void OnSoundButton() {
		localSettings.SoundOn	= !localSettings.SoundOn;
		SoundButton.Toggle(localSettings.SoundOn);
		GameResources.Instance.SaveLocalSettings();
	}

	public void OnMusicButton() {
		localSettings.MusicOn	= !localSettings.MusicOn;
		MusicButton.Toggle(localSettings.MusicOn);
		GameResources.Instance.SaveLocalSettings();
	}
}
