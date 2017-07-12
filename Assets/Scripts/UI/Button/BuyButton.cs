using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	public void Init(UserAssetType? type, int? amount, string name, Color? textColor = null) {
		if(type != null) {
			Image img = UnityUtill.FindByName(transform, "PriceType").GetComponent<Image>();
			img.sprite = GameObjectResources.GetUserAssetIcone(type.Value);
		}

		if(amount != null) {
			bool forNothing = false;
			if(type != null && type == UserAssetType.Energy) {
				forNothing = GameResources.Instance.GetUserData().InfinityEnergyDuration > 0;
			}
			Text text = UnityUtill.FindByName(transform, "PriceAmount").GetComponent<Text>();
			text.text = (forNothing)? "0" : amount.ToString();
			if(textColor != null) {
				text.color = textColor.Value;
			} else if(type != null) {
				text.color = type.Value.ToColor();
			}

		}

		if(name != null) {
			Text text = UnityUtill.FindByName(transform, "Name").GetComponent<Text>();
			text.text = name;
		}

		UpdateLayout();
	}

	public void UpdateLayout() {
		Transform pi = UnityUtill.FindByName(transform, "PriceItem");
		if(pi != null) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(pi.GetComponent<RectTransform>());
		} else {
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
		}
	}
}
