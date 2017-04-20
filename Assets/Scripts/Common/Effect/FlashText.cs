using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashText : FlashObject {

	private Text text;

	protected override void Start() {
		text = GetComponent<Text>();
		base.Start();
	}
	
	protected override Color GetColor() {
		return text.color;
	}

	protected override void SetColor(Color c) {
		text.color = c;
	}

	public void SetText(string newText, bool forceFlash = false) {
		if(text.text != newText || forceFlash) {
			text.text = newText;
			Flash();
		}
	}
}
