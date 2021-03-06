﻿using System;
using UnityEngine;

namespace Common.Animation {
public class AFade : ABase {
	protected float? startAlpha;
	private float endAlpha;
	private float time;
	private float initTime;

	private float startTime;
	private float startDurationTime;
	private bool isComplete;

	public AFade(float? startAlpha, float endAlpha, float time) {
		this.startAlpha = startAlpha;
		this.endAlpha = endAlpha;
		this.time = time;
		this.initTime = time;
	}

	public override void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}
	public override void Reset() {
		time = initTime;
	}

	public override bool Animate(GameObject gameObject) {
		if(startAlpha == null) {
			SetStartAlpha(gameObject);
		}

		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

		SetAlpha(gameObject, Mathf.Lerp(startAlpha.Value, endAlpha, t));

		isComplete = !(time > 0);
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

	virtual protected void SetStartAlpha(GameObject gameObject) {
		SpriteRenderer renderer = Preconditions.NotNull(gameObject.GetComponent<SpriteRenderer>(), "There is no 'SpriteRenderer' attached to the {0}", gameObject.name);
		startAlpha = renderer.material.color.a;
	}

	public override bool IsCompleteAnimation() {
		return isComplete;
	}
}
}

