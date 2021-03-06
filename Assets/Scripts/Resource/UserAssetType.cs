﻿using System;
using UnityEngine;


public enum UserAssetType {
	Money, Energy, Mobile, Ring, Star
}

public static class UserAssetTypeExtension {

	public static Color ToColor(this UserAssetType type){
		switch (type) {
		case UserAssetType.Money:
			return new Color(0.7f, 1, 0.7f); //179 255 179
		case UserAssetType.Energy:
			return new Color(1, 1, 1);
		case UserAssetType.Ring:
			return new Color(1f, 0.84f, 0f);  // 255 214 0 
		case UserAssetType.Mobile:
			return new Color(0.65f, 0.73f, 0.85f);
		case UserAssetType.Star:
			return new Color(0.87f, 0.77f, 0.66f);
		}

		return Color.white;
	}

	public static Color ExperienceColor = new Color(0.9f, 0.56f, 0.16f);

}


