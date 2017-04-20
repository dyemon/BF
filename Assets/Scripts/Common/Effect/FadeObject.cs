using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FadeObject : MonoBehaviour {
	public float FadeInSpeed = 1.0f;
	public float FadeOutSpeed = 1.0f;
	public float MinAlpha = 0;
	public float MaxAlpha = 1.0f;

	protected abstract Color GetColor();
	protected abstract void SetColor(Color c);

	protected virtual void Start() {
		Hide();
	}

	public void Hide() {
		Color c = GetColor();
		c.a = MinAlpha;
		SetColor(c);
	}

	protected IEnumerator FadeIn() {
		Color c = GetColor();

		while (c.a < MaxAlpha) {
			yield return null;
			c.a += FadeInSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MaxAlpha;
		SetColor(c);
	}


	protected IEnumerator FadeOut() {
		Color c = GetColor();

		while (c.a > MinAlpha) {
			yield return null;
			c.a -= FadeOutSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MinAlpha;
		SetColor(c);
	}
}
