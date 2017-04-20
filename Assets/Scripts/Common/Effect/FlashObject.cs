using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlashObject : MonoBehaviour {

	public float FadeInSpeed = 1.0f;
	public float FadeOutSpeed = 1.0f;
	public float MinAlpha = 0;
	public float MaxAlpha = 1.0f;
	public float VisibleTime = 1;
	public float HideTime = 1;
	public bool Continue = false;

	protected abstract Color GetColor();
	protected abstract void SetColor(Color c);

	protected virtual void Start() {
		Hide();
	}

	public void Flash() {
		StartCoroutine(FadeIn());
	}

	public void Hide() {
		Color c = GetColor();
		c.a = MinAlpha;
		SetColor(c);
	}

	private IEnumerator FadeIn() {
		Color c = GetColor();

		while (c.a < MaxAlpha) {
			yield return null;
			c.a += FadeInSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MaxAlpha;
		SetColor(c);

		yield return new WaitForSeconds(VisibleTime);
		StartCoroutine(FadeOut());
	}


	private IEnumerator FadeOut() {
		Color c = GetColor();

		while (c.a > MinAlpha) {
			yield return null;
			c.a -= FadeOutSpeed * Time.deltaTime;
			SetColor(c);
		}

		c.a = MinAlpha;
		SetColor(c);

		if(Continue) {
			yield return new WaitForSeconds(HideTime);
			StartCoroutine(FadeIn());
		}
	}
}