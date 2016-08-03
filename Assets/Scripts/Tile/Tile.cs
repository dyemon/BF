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

	public bool IsAvaliable {
		get {
			return Type != TileType.Unavaliable && (tileItem == null || tileItem.IsAvaliable());
		}
	}

	public bool IsEmpty {
		get {
			return IsAvaliable && tileItem == null;
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
		Predicates.NotNull(tileItem, "Can not get Animatedobject for null object");
		return GetTileItemGO().GetComponent<AnimatedObject>();
	}
}
