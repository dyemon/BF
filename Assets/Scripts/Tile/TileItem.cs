using UnityEngine;
using System.Collections;

public class TileItem {
	public const int BOMBH_OFFSET = 1;
	public const int BOMBV_OFFSET = 2;
	public const int ENVELOP_OFFSET = 3;
	public const int BOMBHV_OFFSET = 4;

	public const int BOMBALL_OFFSET = 5;

	public const int BRILLIANT_OFFSET = 0;


	private TileItemType type;
	private TileItemTypeGroup typeGroup;
	private GameObject tileItemGO;


	private TileItemController itemController;
	private TileItem transitionTileItem;

	public int Level { get; set; }

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
		itemController = go.GetComponent<TileItemController>();		
		Level = 0;
	}

	public static bool IsNotStaticItem(TileItemType type) {
		return IsNotStaticItem(TypeToTypeGroup(type));
	}
	public static bool IsNotStaticItem(TileItemTypeGroup typeGroup) {
		return typeGroup != TileItemTypeGroup.Static;
	}

	public bool IsNotStatic {
		get {return IsNotStaticItem(TypeGroup);}
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
		SetRenderState(TileItemRenderState.HighLight);

		SetTransitionTileItem(transitionTileItem);
	}

	public void SetTransitionTileItem(TileItem transitionTileItem) {
		if(transitionTileItem != null && transitionTileItem.itemController != null) {
			transitionTileItem.itemController.SetTransition(GetGameObject());
			this.transitionTileItem = transitionTileItem;
		}

		if(transitionTileItem == null && this.transitionTileItem != null) {
			this.transitionTileItem.itemController.UnsertTransition();
			this.transitionTileItem = null;
		}
	}

	public void UnSelect(TileItemRenderState state) {
		SetRenderState(state);
		SetTransitionTileItem(null);
	}

	public static TileItemTypeGroup TypeToTypeGroup( TileItemType type) {
		return (TileItemTypeGroup)(type - (int)type % 20);
	}
		
	public static int TypeToIndex( TileItemType type) {
		return (int)type % 20;
	}

	public bool IsMoved { get; set;}

	public static bool IsColorItem(TileItemType type) {
		return (int)type < (int)TileItemTypeGroup.Static; 
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

	public bool IsBombH {
		get { 
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBH_OFFSET;
		}
	}

	public bool IsBombV {
		get { 
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBV_OFFSET;
		}
	}

	public bool IsBombHV {
		get { 
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBHV_OFFSET;
		}
	}

	public bool IsBombAll {
		get {
			return type == TileItemType.BombAll;
		}
	}

	public bool IsBomb {
		get {
			return IsBombH || IsBombV || IsBombHV;
		}
	}

	public bool IsBreakableOnlyByBomb {
		get {
			return IsBombAll;
		}
	}

	public static bool IsRepositionItem(TileItemType type) {
		return IsNotStaticItem(type) && type != TileItemType.BombAll;
	} 

	public bool IsReposition {
		get {
			return IsRepositionItem(type);
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
		return (itemController == null)? 1 : itemController.Damage(damage);
	}

	public void SetRenderState(TileItemRenderState state) {
		if(itemController != null) {
			itemController.SetRenderState(state);
		}

	}

	public static TileItem Instantiate(TileItemType type, GameObject go) {
		/*switch(type) {
			case TileItemType.Unavaliable_2:
				return new BreakableTileItem(type, go, 1);
		}*/

		return new TileItem(type, go);
	}

	public void Mark(bool isMark) {
		if(itemController != null) {
			itemController.Mark(isMark);
		}
	}

	public int GetEnvelopReplaceItemCount() {
		Preconditions.Check(Level > 0, "Level of envelop must be greater zero");
		return Level * 4 + 4;
	}
}
