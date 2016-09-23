using UnityEngine;
using System.Collections;

public class TileItem {
	public const int BOMB_OFFSET = 1;
	public const int ENVELOP_OFFSET = 2;
	public const int BRILLIANT_OFFSET = 0;

	private TileItemRenderState state;

	private TileItemType type;
	private TileItemTypeGroup typeGroup;
	private GameObject tileItemGO;
	private Color startColor;
	private bool selected = false;
	private bool moved = false;

	private TileItemController itemController;
	private TileItem transitionTileItem;

	public TileItem(TileItemTypeGroup typeGroup, int index, GameObject go) {
		SetType(typeGroup, index);
		init(go);
	}
	public TileItem(TileItemType type, GameObject go) {
		SetType(type);
		init(go);
	}

	private void init(GameObject go) {
		tileItemGO = go;
		startColor = go.GetComponent<SpriteRenderer>().color;
		itemController = go.GetComponent<TileItemController>();		
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

	public void Select(TileItem transitionTileItem) {
		selected = true;
		SetRenderState(TileItemRenderState.HighLight);

		if(transitionTileItem != null && transitionTileItem.itemController != null) {
			transitionTileItem.itemController.SetTransition(GetGameObject());
			this.transitionTileItem = transitionTileItem;
		}
	}
	public void UnSelect(TileItemRenderState state) {
		selected = false;
		SetRenderState(state);
		if(transitionTileItem != null) {
			transitionTileItem.itemController.UnsertTransition();
			transitionTileItem = null;
		}
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

	public static bool MayBeFirstItem(TileItemType type) {
		if(!IsColorItem(type)) {
			return false;
		}
		return TypeToIndex(type) != ENVELOP_OFFSET;
	}
	public bool MayBeFirst {
		get { return MayBeFirstItem(Type); }
	}

	public static bool IsSpecialCollectItem(TileItemType type) {
		return type == TileItemType.Brilliant;
	}
	public bool IsSpecialCollect {
		get { return IsSpecialCollectItem(Type); }
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
		
	virtual public int Damage(int damage) {
		return 1;
	}

	public void SetRenderState(TileItemRenderState state) {
		this.state = state;

		if(itemController != null) {
			itemController.SetRenderState(state);
		}

	}

	public static TileItem Instantiate(TileItemType type, GameObject go) {
		switch(type) {
			case TileItemType.Unavaliable_2:
				return new BreakableTileItem(type, go, 1);
		}

		return new TileItem(type, go);
	}


}
