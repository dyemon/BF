using UnityEngine;
using System.Collections;

public class AutoDropTileItems {
	private AutoDropData[] data;
	private bool[] droped;
	private bool dropAll = false;

	public AutoDropTileItems(AutoDropData[] data) {
		if(data != null && data.Length > 0) {
			this.data = data;
			droped = new bool[data.Length];
			ReseteDroped();
		}
	}

	public void ReseteDroped() {
		if(droped == null) {
			return;
		}

		for(int i = 0; i < droped.Length;i++) {
			droped[i] = false;
		}
		dropAll = false;
	}

	public TileItemData GetDropeItem() {
		if(data == null || data.Length == 0 || dropAll) {
			return null;
		}

		int index = Random.Range(0, data.Length);
		if(droped[index]) {
			return null;
		}

		AutoDropData choice = data[index];

		if(Random.Range(0, choice.Ratio) == 0) {
			droped[index] = true;
			dropAll = true;
			foreach(bool item in droped) {
				if(!item) {
					dropAll = false;
					break;
				}
			}
			return choice.TileItem;
		}

		return null;
	} 
}
