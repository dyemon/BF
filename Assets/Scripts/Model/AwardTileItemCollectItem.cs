using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AwardTileItemCollectItem {
	public string TileItemTypeAsString;
	public TileItemType TileItemType;
	public string AwardTypeAsString;
	public UserAssetType AwardType;
	public int Count;

	public void Init() {
		TileItemType = EnumUtill.Parse<TileItemType>(TileItemTypeAsString);
		AwardType = EnumUtill.Parse<UserAssetType>(AwardTypeAsString);
	}
}
