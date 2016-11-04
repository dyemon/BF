using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ModalPanel : MonoBehaviour {

	public Text question;
	public Button yesButton;
	public Button noButton;
	public Button cancelButton;

	void Start() {
		gameObject.SetActive(false);
	}


	// Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
	public void Show (string text, UnityAction yesEvent, UnityAction noEvent, UnityAction closeEvent) {
		gameObject.SetActive (true);

		if(yesButton != null) {
			yesButton.onClick.RemoveAllListeners();
			if(yesEvent != null) {
				yesButton.onClick.AddListener(yesEvent);
			}
			yesButton.onClick.AddListener(ClosePanel);
		}

		if(noButton != null) {
			noButton.onClick.RemoveAllListeners();
			if(noEvent != null) {
				noButton.onClick.AddListener(noEvent);
			}
			noButton.onClick.AddListener(ClosePanel);
		}

		if(cancelButton != null) {
			cancelButton.onClick.RemoveAllListeners();
			if(closeEvent != null) {
				cancelButton.onClick.AddListener(closeEvent);
			}
			cancelButton.onClick.AddListener(ClosePanel);
		}

		if(this.question != null && text != null) {
			this.question.text = text;
		}

	}

	public void ClosePanel () {
		gameObject.SetActive (false);
	}
}
