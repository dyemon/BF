using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileItemData {
	public int X;
	public int Y;
	public TileItemType Type;
	public string TypeAsString;
	public int Level = 1;

	public TileItemData(int x, int y, TileItemType type) {
		this.X = x;
		this.Y = y;
		this.Type = type;
	}


}
