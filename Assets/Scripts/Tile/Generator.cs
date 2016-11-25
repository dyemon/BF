using UnityEngine;
using System.Collections;

public class Generator {
	public int X;
	public int Y;
	private ChildTileItemData data;
	private int currentRatio;

	public Generator(int x, int y, ChildTileItemData data) {
		X = x;
		Y = y;
		this.data = data;
		currentRatio = 0;
	}

	public TileItemData Generate() {
		if(data.Ratio == 0) {
			return null;
		}

		if(++currentRatio == data.Ratio) {
			TileItemData tiData = new TileItemData(X, Y, data.Type);
			tiData.Level = data.Level;
			tiData.Health = data.Health;
			currentRatio = 0;
			return tiData;
		}

		return null;
	}
}
