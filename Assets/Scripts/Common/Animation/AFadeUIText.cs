using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Common.Animation {
	
public class AFadeUIText : AFade {

	public AFadeUIText(float? startAlpha, float endAlpha, float time) : base(startAlpha, endAlpha, time) {
	}

	override protected void SetAlpha(GameObject gameObject, float alpha) {
		Text text = gameObject.GetComponent<Text>();
		Color color = text.color;
		color.a = alpha;
		text.color = color;
	}

	override protected void SetStartAlpha(GameObject gameObject) {
		Text text = Preconditions.NotNull(gameObject.GetComponent<Text>(), "There is no 'Text' attached to the {0}", gameObject.name);
		startAlpha = text.color.a;
	}
		
}
}