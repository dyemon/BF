using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CanvasGroup))]
public class FadeCanvasGroup : FadeObject {
	private CanvasGroup canvasGroup;

	protected override void Start () {
		canvasGroup = GetComponent<CanvasGroup>();
		base.Start();
	}

	protected override Color GetColor() {
		return new Color(1, 1, 1, canvasGroup.alpha);
	}

	protected override void SetColor(Color c) {
		canvasGroup.alpha = c.a;
	}

}
