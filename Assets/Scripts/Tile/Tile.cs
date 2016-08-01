using UnityEngine;
using System.Collections;
using System;

public class Tile : ICloneable {
	private TileType type = TileType.Avaliable;
	public TileType Type { get; set;}

	private int x;
	public int X { get; set;}
	private int y;
	public int Y { get; set;}

	private TileItem tileItem;

	public Tile(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public bool IsAvaliable() {
		return type != TileType.NotAvaliable && (tileItem == null || tileItem.IsAvaliable());
	}

	public bool IsEmpty() {
		return IsAvaliable() && tileItem == null;
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
