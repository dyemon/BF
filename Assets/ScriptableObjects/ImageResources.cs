using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/ImageResources")]
public class ImageResources : ScriptableObject {
	public Sprite[] UserAssets;

	public Sprite GetUserAsset(UserAssetType type) {
		return UserAssets[(int)type];
	}
}
