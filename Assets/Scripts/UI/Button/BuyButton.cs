using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour {
	public GameObjectResources GameObjectResources;
	public bool ConsiderInfinityEnergy = false;

	private UserAssetType? currentType;
	private  Color? currentTextColor;
	private int? currentAmount;

	void OnEnable() {
		GameResources.Instance.onUpdateInfinityEnergy += OnUpdateInfinityEnergy;
	}

	void OnDisable() {
		GameResources.Instance.onUpdateInfinityEnergy -= OnUpdateInfinityEnergy;
	}

	public void Init(UserAssetType? type, int? amount, string name, Color? textColor = null) {
		currentType = type;
		currentTextColor = textColor;
		currentAmount = amount;

		if(type != null) {
			Image img = UnityUtill.FindByName(transform, "PriceType").GetComponent<Image>();
			img.sprite = GameObjectResources.GetUserAssetIcone(type.Value);
		}

		UpdateText(amount);

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

	void UpdateText(int? amount) {
		if(amount != null) {
			bool forNothing = false;
			if(ConsiderInfinityEnergy && currentType != null && currentType == UserAssetType.Energy) {
				forNothing = GameResources.Instance.GetUserData().InfinityEnergyDuration > 0;
			}
			Text text = UnityUtill.FindByName(transform, "PriceAmount").GetComponent<Text>();
			text.text = (forNothing)? "0" : amount.ToString();
			if(currentTextColor != null) {
				text.color = currentTextColor.Value;
			} else if(currentType != null) {
				text.color = currentType.Value.ToColor();
			}

		}
	}

	public void UpdateAmountFromText() {
		Text text = UnityUtill.FindByName(transform, "PriceAmount").GetComponent<Text>();
		currentAmount = System.Int32.Parse(text.text);
	}

	public void OnUpdateInfinityEnergy(int value) {
		if(!ConsiderInfinityEnergy || currentType == null || currentType.Value != UserAssetType.Energy) {
			return;
		}

		UpdateText(value);
		UpdateLayout();
	}
		
	public PriceItem GetPriceItem() {
		if(currentType == null || currentAmount == null) {
			return null;
		}

		return new PriceItem() { Type = currentType.Value, Value = currentAmount.Value };
	}
}
