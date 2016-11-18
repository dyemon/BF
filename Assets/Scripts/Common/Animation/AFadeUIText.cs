using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AFadeUIText : AFade {

	public AFadeUIText(float startAlpha, float endAlpha, float time) : base(startAlpha, endAlpha, time){
	}

	override protected void SetAlpha(GameObject gameObject, float alpha ) {
		Text text = gameObject.GetComponent<Text>();
		Color color = text.color;
		color.a = alpha;
		text.color = color;
	}
		
}
