﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
	public GameObject Progress;
	public GameObject EvaluteProgress;
	public Text text;
	public float TimeToSetProgressComplete = 1;
	public bool ShowPercent;
	public string TextFormat;
	public Vector3 TextOffset = Vector2.zero;

	private int curValue;
	public int MaxValue; 
	private bool notChangeEvalute = false;
	private int curEvaluteOffset;
	// Use this for initialization
	void Start () {
		
	}

	public bool SetProgress(int i, bool isAnimate){
		if(MaxValue <= 0) {
			return false;
		}
		int oldValue = curValue;
		if(i > MaxValue) {
			i = MaxValue;
		}
		curValue = i;

		if(i == 0 || !isAnimate) {
			Progress.GetComponent<Renderer>().material.SetFloat("_Progress", i);
			UpdateText();
		} else {
			StartCoroutine(AnimateProgress(oldValue));
		}

		return i >= MaxValue;
	}

	public bool SetEvaluteProgress(int i){
		if(MaxValue <= 0) {
			return false;
		}
		if(curValue + i > MaxValue) {
			i = MaxValue - curValue;
		}

		EvaluteProgress.GetComponent<Renderer>().material.SetFloat("_Progress", ((float)(curValue +i))/MaxValue);

		curEvaluteOffset = i;
		UpdateText();
		return (curValue + i) >= MaxValue;
	}

	public bool IncreesProgress(int point, bool isAnimate) {
		return SetProgress(curValue + point, isAnimate);
	}

	IEnumerator AnimateProgress(int oldValue) {
		float rate = 1 / TimeToSetProgressComplete;
		float maxVal = ((float)curValue) / MaxValue;
		float i = ((float)oldValue) / MaxValue;
	//	notChangeEvalute = true;

		while (i <= maxVal) {
			i += Time.deltaTime * rate;
			Progress.GetComponent<Renderer>().material.SetFloat("_Progress", i);
			yield return 0;
		}

		UpdateText();
	//	notChangeEvalute = false;
	}

	public bool ChangeMaxValue(int newValue) {
		MaxValue = newValue;
		SetEvaluteProgress(curEvaluteOffset);
		SetProgress(curValue, false);

		return curValue >= MaxValue;
	}

	public void OnResize() {
		if(text == null) {
			return;
		}
	
		Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position + TextOffset);
		text.gameObject.transform.position = screenPos;
	}

	public void SetText(string str) {
		if(text != null) {
			text.text = str;
		}
	}

	private void UpdateText() {
		if((!ShowPercent && string.IsNullOrEmpty(TextFormat)) || text == null || MaxValue == 0) {
			return;
		}

		string str;
		if(ShowPercent) {
			float percent = Mathf.Round(((float)(curValue + curEvaluteOffset)) * 100 / MaxValue);
			str = string.Format("{0}%", percent);
		} else {
			str = string.Format(TextFormat, curValue);
		}

		SetText(str);
	}

	public void SetMaxValue(int val) {
		MaxValue = val;
		UpdateText();
	}
}
