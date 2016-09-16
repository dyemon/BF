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

	public static bool IsAvaliableItem(TileItemType type) {
		return IsAvaliableItem(TypeToTypeGroup(type));
	}
	public static bool IsAvaliableItem(TileItemTypeGroup typeGroup) {
		return typeGroup != TileItemTypeGroup.Unavaliable;
	}

	public bool IsAvaliable() {
		return IsAvaliableItem(TypeGroup);
	}

	private void SetType(TileItemTypeGroup typeGroup, int index) {
		this.typeGroup = typeGroup;
		type = (TileItemType)(typeGroup + index);
	}

	private void SetType(TileItemType type) {
		this.type = type;
		this.typeGroup = TypeToTypeGroup(type);
	}
		


	public GameObject GetGameObject() {
		return tileItemGO;
	}

	public void Select(bool isSelect) {
		selected = isSelect;
		tileItemGO.GetComponent<SpriteRenderer>().color = (selected) ? Color.white : startColor;
	}

	public static TileItemTypeGroup TypeToTypeGroup( TileItemType type) {
		return (TileItemTypeGroup)(type - (int)type % 20);
	}
		
	public static int TypeToIndex( TileItemType type) {
		return (int)type % 20;
	}

	public bool IsMoved { get; set;}

	public static bool IsColorItem(TileItemType type) {
		return (int)type < (int)TileItemTypeGroup.Unavaliable; 
	}

	public bool IsColor {
		get { return TileItem.IsColorItem(type);}
	}
	public bool IsSpecial {
		get { return (int)type >= (int)TileItemTypeGroup.Special; }
	}

	public TileItemTypeGroup TypeGroup {
		get { return typeGroup; }
	}
	public TileItemType Type {
		get { return type; }
	}

	public static bool IsEnvelopItem(TileItemType type) {
		if(!IsColorItem(type)) {
			return false;
		}
		return TypeToIndex(type) == ENVELOP_OFFSET;
	}

	public bool IsEnvelop {
		get { return IsEnvelopItem(Type); }
	}

	public static bool IsSimpleItem(TileItemType type) {
		if(!IsColorItem(type)) {
			return false;
		}
		return TypeToIndex(type) == 0;
	}

	public bool IsSimple {
		get { return IsSimpleItem(Type); }
	}

	public bool IsBomb {
		get { 
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMB_OFFSET;
		}
	}

	public bool IsBrilliant {
		get { 
			if(!IsSpecial) {
				return false;
			}
			return TypeToIndex(type) == BRILLIANT_OFFSET;
		}
	}
		
}
