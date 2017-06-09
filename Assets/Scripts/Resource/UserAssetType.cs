using System;
using UnityEngine;


public enum UserAssetType {
	Money, Semka, Mobile, Ring, Star
}

static class UserAssetTypeExtension {

	public static Color ToColor(this UserAssetType type){
		switch (type) {
		case UserAssetType.Money:
			return new Color(0.7f, 1, 0.7f);
		case UserAssetType.Semka:
			return new Color(1, 1, 1);
		case UserAssetType.Ring:
			return new Color(1f, 0.84f, 0f);
		case UserAssetType.Mobile:
			return new Color(0.65f, 0.73f, 0.85f);
		case UserAssetType.Star:
			return new Color(1f, 0.5f, 0.5f);
		}

		return Color.white;
	}
}


