using UnityEngine;
using System.Collections;

public class TileItem {
	private TileItemType type;
	private TileItemTypeGroup typeGroup;
	private GameObject tileItemGO;
	private Color startColor;
	private bool selected = false;
	private bool moved = false;

	public TileItem(TileItemTypeGroup typeGroup, int index, GameObject go) {
		SetType(typeGroup, index);
		tileItemGO = go;
		startColor = go.GetComponent<SpriteRenderer>().color;
	}
	public TileItem(TileItemType type, GameObject go) {
		SetType(type);
		tileItemGO = go;
		startColor = go.GetComponent<SpriteRenderer>().color;
	}

	public bool IsAvaliable() {
		return typeGroup != TileItemTypeGroup.Unavaliable;
	}

	private void SetType(TileItemTypeGroup typeGroup, int index) {
		this.typeGroup = typeGroup;
		type = (TileItemType)(typeGroup + index);
	}

	private void SetType(TileItemType type) {
		this.type = type;
		this.typeGroup = TypeToGroupType(type);
	}
		
	public GameObject GetGameObject() {
		return tileItemGO;
	}

	public void ToggleSelect() {
		selected = !selected;
		tileItemGO.GetComponent<SpriteRenderer>().color = (selected) ? Color.white : startColor;
	}

	public static TileItemTypeGroup TypeToGroupType( TileItemType type) {
		return (TileItemTypeGroup)(type - (int)type % 20);
	}

	public void SetMoved(bool moved) {
		this.moved = moved;
	}
	public bool IsMoved() {
		return moved;
	}

}
