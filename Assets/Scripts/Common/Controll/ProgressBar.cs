using System.Collections;
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
	public bool Invers = false;

	private int curValue;
	public int MaxValue; 
	private int curEvaluteOffset;
	private Camera camera;

	private int screenWidth;
	private int screenHeight;

	// Use this for initialization
	void Start () {
		camera = Camera.main;
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

		SetEvaluteProgress(0);
		return i >= MaxValue;
	}

	public bool SetEvaluteProgress(int i) {
		if(MaxValue <= 0 || EvaluteProgress == null) {
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

		Vector3 screenPos = camera.WorldToScreenPoint(gameObject.transform.position + TextOffset);
		text.gameObject.transform.position = screenPos;
	//	Debug.Log(gameObject.transform.position);
	//	Debug.Log(screenPos);
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
			int val = (Invers) ? MaxValue - curValue : curValue;
			if(val < 0) {
				val = 0;
			}
			if(val > MaxValue) {
				val = MaxValue;
			}

			str = string.Format(TextFormat, val);
		}

		SetText(str);
	}

	public void SetMaxValue(int val) {
		MaxValue = val;
		UpdateText();
	}

	void Update() {
	//	if((Screen.width != screenWidth || Screen.height != screenHeight) && camera.isActiveAndEnabled) {
	//		screenWidth = Screen.width;
	//		screenHeight = Screen.height;
			OnResize();
//		}
	}

	public void ShowText(bool show) {
		text.enabled = show;
	}
}
