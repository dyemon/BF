using UnityEngine;
using System.Collections;

public class TileItem {
	public const int BOMB_OFFSET = 1;
	public const int ENVELOP_OFFSET = 2;
	public const int BRILLIANT_OFFSET = 0;

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
		
	public static int TypeToIndex( TileItemType type) {
		return (int)type % 20;
	}

	public bool IsMoved { get; set;}

	public bool IsColor {
		get { return (int)type < (int)TileItemTypeGroup.Unavaliable; }
	}

	public TileItemTypeGroup TypeGroup {
		get { return typeGroup; }
	}
		
}
