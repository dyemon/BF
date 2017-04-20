using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlashObject : FadeObject {


	public float VisibleTime = 1;
	//public float HideTime = 1;
	//public bool Continue = false;

	public void Flash() {
		StartCoroutine(FlashInternal());
	}

	private IEnumerator FlashInternal() {
		StartCoroutine(FadeIn());
		yield return new WaitForSeconds(VisibleTime);
		StartCoroutine(FadeOut());
	}


}