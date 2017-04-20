using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMultiplierText : FlashText {
	float value = 1;

	public void Reset() {
		value = 1;
	}

	public void SetValue(float val) {
		if(val > value) {
			SetText(string.Format("x{0}", val), true);
		}
		value = val;
	}
}
