using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButton : MonoBehaviour {

	public Image OffImage;
	private bool onState;

	public void Toggle(bool onState) {
		OffImage.enabled = !onState;
		this.onState = onState;
	}

	public bool IsOn() {
		return onState;
	}

}
