using UnityEngine;
using System.Collections;
using System;

public class Tile : ICloneable {
	public TileType Type { get; set;}
	public int X { get; set; }
	public int Y { get; set; }

	private TileItem tileItem;

	public Tile(int x, int y) {
		this.X = x;
		this.Y = y;
	}

	public static bool IsAvaliableTile(TileType type) {
		return type != TileType.Unavaliable;
	}

	public bool IsAvaliable {
		get {
			return IsAvaliableTile(Type) && (tileItem == null || tileItem.IsNotStatic());
		}
	}

	public bool IsEmpty {
		get {
			return IsAvaliable && tileItem == null;
		}
	}

	public bool IsColor {
		get {
			return IsAvaliable && tileItem != null && tileItem.IsColor;
		}
	}
	public bool IsSimple {
		get {
			return IsAvaliable && tileItem != null && tileItem.IsSimple;
		}
	}

	public void SetTileItem(TileItem ti) {
		tileItem = ti;
	}

	public GameObject GetTileItemGO() {
		return tileItem.GetGameObject();
	}
	public TileItem GetTileItem() {
		return tileItem;
	}

	public object Clone() {
		return this.MemberwiseClone();
	}

	public AnimatedObject GetAnimatedObject() {
		Preconditions.NotNull(tileItem, "Can not get Animatedobject for null object");
		return GetTileItemGO().GetComponent<AnimatedObject>();
	}

	public override string ToString() {
		return string.Format("[Tile: Type={0}, X={1}, Y={2}, IsAvaliable={3}, IsEmpty={4}]", Type, X, Y, IsAvaliable, IsEmpty);
	}

	public TileItemType TileItemType {
		get {
			return (IsAvaliableTile(Type) && tileItem != null) ? tileItem.Type : TileItemType.Static_1;
		}
	}
}
