using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour {
	public GameObjectResources GameObjectResources;

	public void Init(UserAssetType? type, int? amount, string name) {
		if(type != null) {
			Image img = UnityUtill.FindByName(transform, "Price Type").GetComponent<Image>();
			img.sprite = GameObjectResources.GetUserAssetIcone(type.Value);
		}

		if(amount != null) {
			Text text = UnityUtill.FindByName(transform, "Price Amount").GetComponent<Text>();
			text.text = amount.ToString();
		}

		if(name != null) {
			Text text = UnityUtill.FindByName(transform, "Name").GetComponent<Text>();
			text.text = name;
		}
	}

}
