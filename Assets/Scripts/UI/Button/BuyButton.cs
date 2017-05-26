using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	protected void Init(Sprite bg, Sprite icon, string buttonText,UserAssetType priceType, int priceValue) {
		SetPrice(priceType, priceValue);
		SetBg(bg);
		SetIcon(icon);
		SetButtonText(buttonText);
	}

	protected void SetPrice(UserAssetType priceType, int priceValue) {
		Image img = (Image)transform.FindChild("Button/Price Image").GetComponent<Image>();
		if(img != null) {
			img.sprite = GameObjectResources.GetUserAssetIcone(priceType);
		}
		Text text = transform.FindChild("Button/Price Text").GetComponent<Text>();
		if(text != null) {
			text.text = priceValue.ToString();
		}
	}

	protected void SetBg(Sprite bg) {
		Image img = (Image)transform.FindChild("Button").GetComponent<Image>();
		if(img != null) {
			img.sprite = bg;
		}
	}

	protected void SetIcon(Sprite icon) {
		Image img = (Image)transform.FindChild("Icon").GetComponent<Image>();
		if(img != null) {
			img.sprite = icon;
		}
	}

	protected void SetButtonText(string str) {
		Text text = transform.FindChild("Button/Text").GetComponent<Text>();
		if(text != null) {
			text.text = str.ToString();
		}
	}
}
