using UnityEngine;
using System.Collections;

public class TileItem {
	private TileItemType type;
	private TileItemTypeGroup typeGroup;
	private GameObject tileItemGO;

	public TileItem(TileItemTypeGroup typeGroup, int index, GameObject go) {
		SetType(typeGroup, index);
		tileItemGO = go;
	}

	public bool IsAvaliable() {
		return typeGroup != TileItemTypeGroup.NotAvaliable;
	}

	private void SetType(TileItemTypeGroup typeGroup, int index) {
		this.typeGroup = typeGroup;
		type = (TileItemType)(typeGroup + index);
	}

	public GameObject GetGameObject() {
		return tileItemGO;
	}
}
