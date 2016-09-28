using UnityEngine;
using System.Collections;

public class TileItemData {
	public int X;
	public int Y;
	public TileItemType Type;
	public int Level;

	public TileItemData(int x, int y, TileItemType type) {
		this.X = x;
		this.Y = y;
		this.Type = type;
		Level = 0;
	}


}
