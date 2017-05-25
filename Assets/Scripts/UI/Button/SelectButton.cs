using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	protected void Init(Sprite bg, Sprite icon, string buttonText,UserAssetType priceType, int priceValue) {
		SetPrice(priceType, priceValue);
		SetBg(bg);
		SetIcon(icon);
		SetButtonText(buttonText);
	}

	protected void SetPrice(UserAssetType priceType, int priceValue) {
		Image img = (Image)transform.FindChild("Button/Price Image").GetComponent<Image>();
		img.sprite = GameObjectResources.GetUserAssetIcone(priceType);
		Text text = transform.FindChild("Button/Price Text").GetComponent<Text>();
		text.text = priceValue.ToString();
	}

	protected void SetBg(Sprite bg) {
		Image img = (Image)transform.FindChild("Button").GetComponent<Image>();
		img.sprite = bg;
	}

	protected void SetIcon(Sprite icon) {
		Image img = (Image)transform.FindChild("Icon").GetComponent<Image>();
		img.sprite = icon;
	}

	protected void SetButtonText(string str) {
		Text text = transform.FindChild("Button/Text").GetComponent<Text>();
		text.text = str.ToString();
	}
}
