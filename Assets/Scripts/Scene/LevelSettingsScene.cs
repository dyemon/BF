using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelSettingsScene : MonoBehaviour {

	private ModalPanel modalPanel;
	private DisplayManager displayManager;

	private UnityAction yesAction;
	private UnityAction cancelAction;

	void Awake () {
		modalPanel = ModalPanel.Instance ();
		displayManager = DisplayManager.Instance ();

		yesAction = new UnityAction (OnCapitulateYes);
		cancelAction = new UnityAction (OnCapitulateCancel);
	}

	public void OnCapitulate () {
		modalPanel.Choice ("Вы действительно хотите сдаться?", OnCapitulateYes, OnCapitulateCancel);
	}

	public void OnCapitulateYes () {
		SceneController.Instance.LoadSceneAsync("Start");
	}
		
	public void OnCapitulateCancel () {
		modalPanel.ClosePanel();
	}

}
