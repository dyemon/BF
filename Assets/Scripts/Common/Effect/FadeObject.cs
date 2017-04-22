using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FadeObject : MonoBehaviour {
	public float FadeInSpeed = 3.0f;
	public float FadeOutSpeed = 3.0f;
	public float MinAlpha = 0;
	public float MaxAlpha = 1.0f;
	public bool IsFadeInOnStart = false;

	protected abstract Color GetColor();
	protected abstract void SetColor(Color c);

	public bool IsDone {get; set;}

	protected virtual void Start() {
		Hide();
		if(IsFadeInOnStart) {
			FadeIn();
		}
	}

	public void Hide() {
		Color c = GetColor();
		c.a = MinAlpha;
		SetColor(c);
	}

	public void FadeIn() {
		IsDone = false;
		StartCoroutine(FadeInCoroutine());
	}

	public void FadeOut() {
		IsDone = false;
		StartCoroutine(FadeOutCoroutine());
	}

	private IEnumerator FadeInCoroutine() {
		Color c = GetColor();

		while (c.a < MaxAlpha) {
			yield return null;
			c.a += FadeInSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MaxAlpha;
		SetColor(c);
		IsDone = true;
	}


	private IEnumerator FadeOutCoroutine() {
		Color c = GetColor();

		while (c.a > MinAlpha) {
			yield return null;
			c.a -= FadeOutSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MinAlpha;
		SetColor(c);
		IsDone = true;
	}
}
