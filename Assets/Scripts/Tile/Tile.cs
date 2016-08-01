using UnityEngine;
using System.Collections;
using System;

public class Tile : ICloneable {
	private TileType type = TileType.Avaliable;
	public TileType Type { get; set;}

	private GameObject goTileItem;
	private TileItem tileItem;

	public bool isAvaliable() {
		return type != TileType.NotAvaliable;
	}

	public bool IsEmpty() {
		return isAvaliable() && tileItem != null;
	}

	public void SetTileItem(GameObject ti) {
		goTileItem = ti;
		tileItem = (ti == null)? null : ti.GetComponent<TileItem>();
	}

	public object Clone() {
		return this.MemberwiseClone();
	}
}
