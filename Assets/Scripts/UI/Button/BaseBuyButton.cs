using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseBuyButton<T> : MonoBehaviour {
	public delegate void OnClickListener<T>(T param);

	public GameObjectResources GameObjectResources;

	private T retVal;
	private OnClickListener<T> onClickListener;
	protected void Init(Sprite bg, Sprite icon, string buttonText,UserAssetType priceType, int priceValue, T retVal, OnClickListener<T> onClickListener) {
		this.retVal = retVal;
		this.onClickListener = onClickListener;

		SetPrice(priceType, priceValue);
		SetBg(bg);
		SetIcon(icon);
		SetButtonText(buttonText);

		Button btn = GetButton();
		if(btn != null) {
			btn.onClick.AddListener(OnClick);
		}
	}

	virtual protected Button GetButton() {
		Transform btn = UnityUtill.FindByName(transform, "Button");
		if(btn != null) {
			return btn.GetComponent<Button>();
		}
		return null;
	}

	protected void SetPrice(UserAssetType priceType, int priceValue) {
		Transform img = UnityUtill.FindByName(transform, "Price Image");
		if(img != null) {
			img.GetComponent<Image>().sprite = GameObjectResources.GetUserAssetIcone(priceType);
		}
		Transform text = UnityUtill.FindByName(transform, "Price Text"); 
		if(text != null) {
			text.GetComponent<Text>().text = priceValue.ToString();
			text.GetComponent<Text>().color = priceType.ToColor();
		}
	}

	protected void SetBg(Sprite bg) {
		Transform img = UnityUtill.FindByName(transform, "Bg"); 
		if(img != null && bg != null) {
			img.GetComponent<Image>().sprite = bg;
		}
	}

	protected void SetIcon(Sprite icon) {
		Transform img = UnityUtill.FindByName(transform, "Icon"); 
		if(img != null && icon != null) {
			img.GetComponent<Image>().sprite = icon;
		}
	}

	protected void SetButtonText(string str) {
		Transform text = UnityUtill.FindByName(transform, "Text"); ;
		if(text != null && str != null) {
			text.GetComponent<Text>().text = str.ToString();
		}
	}

	virtual protected void OnClick() {
		onClickListener(retVal);
	}
}
