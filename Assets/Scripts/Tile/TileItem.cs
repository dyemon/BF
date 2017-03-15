using UnityEngine;
using System.Collections;

public class TileItem {
	public const int TILE_ITEM_GROUP_WEIGHT = 100;

	public const int BOMBH_OFFSET = 1;
	public const int BOMBV_OFFSET = 2;
	public const int ENVELOP_OFFSET = 3;
	public const int BOMBHV_OFFSET = 4;
	public const int BOMBP_OFFSET = 5;
	public const int BOMBC_OFFSET = 6;

	public const int BOMBALL_OFFSET = 5;

	public const int BRILLIANT_OFFSET = 0;
	public const int KEY_OFFSET = 1;

	private TileItemType type;
	private TileItemTypeGroup typeGroup;
	private GameObject tileItemGO;


	private TileItemController itemController;
	private TileItem transitionTileItem;

	public int Level { get; set; }

	private TileItem childTileItem;
	private TileItem parentTileItem;
	private ChildTileItemData generatedTileItemData;

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
		Level = 1;
	}

	public static bool IsNotStaticItem(TileItemData data) {
		return data.HasParent() ? IsNotStaticItem(data.ParentTileItemData.Type) : IsNotStaticItem(data.Type);
	}
	private static bool IsNotStaticItem(TileItemType type) {
		TileItemTypeGroup typeGroup = TypeToTypeGroup(type);
		return typeGroup != TileItemTypeGroup.Static && typeGroup != TileItemTypeGroup.Box && typeGroup != TileItemTypeGroup.SpecialStatic;
	}
	public bool IsNotStatic {
		get {
			return parentTileItem != null ? IsNotStaticItem(parentTileItem.Type) : IsNotStaticItem(Type);
		}
	}

	public static bool IsBoxItem(TileItemType type) {
		return TypeToTypeGroup(type) == TileItemTypeGroup.Box;
	}
	public bool IsBox {
		get {return IsBoxItem(type);}
	}


	public void SetType(TileItemTypeGroup typeGroup, int index) {
		this.typeGroup = typeGroup;
		type = (TileItemType)(typeGroup + index);
	}

	public void SetType(TileItemType type) {
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
		return (TileItemTypeGroup)(type - (int)type % TILE_ITEM_GROUP_WEIGHT);
	}
		
	public static int TypeToIndex( TileItemType type) {
		return (int)type % TILE_ITEM_GROUP_WEIGHT;
	}

	public bool IsMoved { get; set;}

	public static bool IsColorItem(TileItemType type) {
		return (int)type < (int)TileItemTypeGroup.Bomb; 
	}
	public bool IsColor {
		get { return TileItem.IsColorItem(type);}
	}

	public static bool IsSpecialItem(TileItemType type) {
		return (int)type >= (int)TileItemTypeGroup.Special; 
	}
	public bool IsSpecial {
		get { return TileItem.IsSpecialItem(type);}
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
		if(IsColorIndependedItem(type)) {
			return true;
		}
		if(!IsColorItem(type)) {
			return false;
		}
		return TypeToIndex(type) != ENVELOP_OFFSET;
	}
	public bool MayBeFirst {
		get { return MayBeFirstItem(Type); }
	}

	public static bool IsSpecialCollectItem(TileItemType type) {
		return type == TileItemType.Brilliant || type == TileItemType.Key;
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

	public bool IsSelectable {
		get { return IsColor || IsColorIndepended; }
	}

	public bool IsBombH {
		get { 
			if(type == TileItemType.BombH) {
				return true;
			}
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBH_OFFSET;
		}
	}

	public bool IsBombV {
		get { 
			if(type == TileItemType.BombV) {
				return true;
			}
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

	public bool IsBombP {
		get { 
			if(type == TileItemType.BombP) {
				return true;
			}
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBP_OFFSET;
		}
	}
	
	public bool IsBombC {
		get { 
			if(type == TileItemType.BombC) {
				return true;
			}
			if(!IsColor) {
				return false;
			}
			return TypeToIndex(type) == BOMBC_OFFSET;
		}
	}
	
	public bool IsBombAll {
		get {
			return type == TileItemType.BombAll;
		}
	}

	public bool IsBomb {
		get {
			return IsBombH || IsBombV || IsBombHV || IsColorIndependedBomb;
		}
	}

	public bool IsBreakableOnlyByBomb {
		get {
			return IsBombAll;
		}
	}

	public bool IsSlime {
		get {
			return type == TileItemType.Slime;
		}
	}

	public bool IsAbsorbable {
		get {
			return IsColor || IsSpecialCollect;
		}
	}

	public bool IsGenerator {
		get {
			return type == TileItemType.GeneratorBlue;
		}
	}

	public bool IsReplacedByGenerator {
		get {
			return (IsSimple || IsSpecialCollect) && IsNotStatic;
		}
	}

	public static bool IsRepositionItem(TileItemData data) {
		return data.HasParent() ? IsRepositionItem(data.ParentTileItemData.Type) : IsRepositionItem(data.Type);
	} 
	private static bool IsRepositionItem(TileItemType type) {
		return IsNotStaticItem(type) && type != TileItemType.BombAll;
	} 
	public bool IsReposition {
		get {
			return parentTileItem != null ? IsRepositionItem(parentTileItem.Type) : IsRepositionItem(Type);
		}
	}
		
	public int Damage(int damage) {
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
	/*
	public void Mark(bool isMark) {
		if(itemController != null) {
			itemController.Mark(isMark);
		}
	}*/

	public int GetEnvelopReplaceItemCount() {
		//Preconditions.Check(Level > 0, "Level of envelop must be greater zero");
		return (Level == 0)? 8 : Level * 4 + 4;
	}

	public void SetStartHealth(int health) {
		if(itemController != null) {
			Preconditions.Check(itemController.SetStartHealth(health), "Health for TileItem {0} can not be {1}", type.ToString(), health);
		}
	}

	public void SetChildTileItem(TileItem tileItem) {
		childTileItem = tileItem;
	}
	public void SetParentTileItem(TileItem tileItem) {
		parentTileItem = tileItem;
	}
	public TileItem GetChildTileItem() {
		return childTileItem;
	}
	public TileItem GetParentTileItem() {
		return parentTileItem;
	}

	public bool DestroyOnBreak() {
		return itemController == null ? true : itemController.DestroyOnBreak();
	}

	public void SetGeneratedData(ChildTileItemData genData) {
		generatedTileItemData = genData;
	}

	public TileItemData GenerateTileItemData() {
		return null;
	}

	public void Rotate() {
		if(itemController != null) {
			itemController.Rotate();
		}
	}
	
	public static bool IsColorIndependedItem(TileItemType type) {
		return (int)type >= (int)TileItemTypeGroup.Bomb && (int)type < (int)TileItemTypeGroup.Static;
	} 
	
	public bool IsColorIndepended {
		get {
			return IsColorIndependedItem(type);
		}
	}
	
	public static bool IsColorIndependedBombItem(TileItemType type) {
		return (int)type >= (int)TileItemTypeGroup.Bomb && (int)type < (int)TileItemTypeGroup.Static;
	} 
	
	public bool IsColorIndependedBomb {
		get {
			return IsColorIndependedBombItem(type);
		}
	}
	
	public bool IsRotateOnDrop {
		get {
			return !IsBomb && !IsBombAll;
		}
	}
}
