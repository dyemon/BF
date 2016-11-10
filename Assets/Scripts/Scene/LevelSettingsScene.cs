using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelSettingsScene : MonoBehaviour {

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

}
