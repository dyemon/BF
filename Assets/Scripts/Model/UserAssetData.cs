using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class UserAssetData {
	public UserAssetData(UserAssetType type, int value) {
		Type = type;
		Value = value;
		TypeAsString = type.ToString();
	}

	[NonSerialized]
	public UserAssetType Type;
	[SerializeField]
	private string TypeAsString;
	public int Value;

	public void Init() {
		Type = (UserAssetType)Enum.Parse(typeof(UserAssetType), TypeAsString);
	}
}
