using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class LevelSettingsScene : WindowScene {
	public OnOffButton SoundButton;
	public OnOffButton MusicButton;
	public Text Description;

	private LocalSettingsData localSettings;

	void Start() {
		localSettings = GameResources.Instance.GetLocalSettings();
		SoundButton.Toggle(localSettings.SoundOn);
		MusicButton.Toggle(localSettings.MusicOn);

		LevelData levelData = GameResources.Instance.GetLevel(App.GetCurrentLevel());
		Description.text = App.GetCurrentLevel().ToString() + ": " + levelData.Name;
	}

	public void OnCapitulate () {
		ModalPanels.Show(ModalPanelName.ConfirmPanel, "Вы действительно хотите сдаться?", OnCapitulateYes, null, null);
	}

	public void OnCapitulateYes () {
		GameResources.Instance.SaveUserData(true);
		SceneController.Instance.LoadSceneAsync("Map");
	}
		
	public void OnCapitulateCancel () {
		ModalPanels.ClosePanel(ModalPanelName.ConfirmPanel);
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
