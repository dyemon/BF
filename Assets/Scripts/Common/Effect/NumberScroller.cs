using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class NumberScroller : MonoBehaviour {

	Text text;
	bool run = false;
	int? startValue = null;
	int endValue;
	float maxDuration = 2;
	float speed = 1;

	float curTime;
	float duration;

	void Start () {
		text = GetComponent<Text>();
	}

	public NumberScroller StartValue(int val) {
		startValue = val;
		return this;
	}
	public NumberScroller EndValue(int val) {
		endValue = val;
		return this;
	}
	public NumberScroller MaxDuration(int val) {
		maxDuration = val;
		return this;
	}
	public NumberScroller Speed(int val) {
		speed = val;
		return this;
	}
	public void Run() {
		run = true;
		curTime = 0;

		if(startValue == null) {
			startValue = System.Int32.Parse(text.text);
		}

		duration = Mathf.Abs((float)endValue - (float)startValue) / speed;
		if(duration > maxDuration) {
			duration = maxDuration;
		}
	}

	void Update() {
		if(!run) {
			return;
		}

		int val;
		curTime += Time.deltaTime;
		if(curTime >= duration) {
			run = false;
			val = endValue;
		} else {
			float ratio = curTime / duration;
			val = (int)Mathf.Round(startValue.Value * (1f - ratio) + endValue * ratio);
		}

		text.text = val.ToString();
	}
}
