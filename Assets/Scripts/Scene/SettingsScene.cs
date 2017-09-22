using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class SettingsScene : WindowScene {
	public const string SceneName = "Settings";

	public OnOffButton SoundButton;
	public OnOffButton MusicButton;

	protected override void Start() {
		base.Start();
		LocalData localData = GameResources.Instance.GetLocalData();
		SoundButton.Toggle(localData.SoundOn);
		MusicButton.Toggle(localData.MusicOn);
	}
		
	public void OnSoundButton() {
		LocalData localData = GameResources.Instance.GetLocalData();
		localData.SoundOn	= !localData.SoundOn;
		SoundButton.Toggle(localData.SoundOn);
		GameResources.Instance.SaveLocalData();
		SoundController.Instance.Enable(localData.SoundOn);
	}

	public void OnMusicButton() {
		LocalData localData = GameResources.Instance.GetLocalData();
		localData.MusicOn	= !localData.MusicOn;
		MusicButton.Toggle(localData.MusicOn);
		GameResources.Instance.SaveLocalData();
		MusicController.Instance.Enable(localData.MusicOn);

	}
}
