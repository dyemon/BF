using System;
using UnityEngine;


public class AFade {
	private float startAlpha;
	private float endAlpha;
	private float time;

	private float startTime;
	private float startDurationTime;

	public AFade(float startAlpha, float endAlpha, float time) {
		this.startAlpha = startAlpha;
		this.endAlpha = endAlpha;
		this.time = time;
	}

	public void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}

	public bool Fade(GameObject gameObject) {
		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

		SetAlpha(gameObject, Mathf.Lerp(startAlpha, endAlpha, t));

		return (time > 0);
	}

	virtual protected void SetAlpha(GameObject gameObject, float alpha ) {
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		if(renderer != null) {
			Color color = renderer.material.color;
			color.a = alpha;
			renderer.material.color = color;
		}
	}
}


