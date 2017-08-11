using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Animation {
	
public class AFadeUI : AFade {
	CanvasGroup group;

	public AFadeUI(float? startAlpha, float endAlpha, float time) : base(startAlpha, endAlpha, time){
	}

	override protected void SetAlpha(GameObject gameObject, float alpha ) {
		if(group == null) {
		}
		group.alpha = alpha;
	}

	override protected void SetStartAlpha(GameObject gameObject) {
		group = Preconditions.NotNull(gameObject.GetComponent<CanvasGroup>(), "There is no 'CanvasGroup' attached to the {0}", gameObject.name);
		startAlpha = group.alpha;
	}
}
}