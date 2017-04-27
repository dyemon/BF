using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileItemSameColorCount {
	public TileItemTypeGroup TypeGroup;
	
	private int itemCount = 0;
	private IDictionary<Vector2, Object> enevlopReplacePositions = new Dictionary<Vector2, Object>();
	public bool MayBeFirst = false;

	public TileItemSameColorCount(TileItemTypeGroup typeGroup) {
		TypeGroup = typeGroup;
	}

	public int Increment() {
		++itemCount;
		return Count();
	}

	public int AddPositions(IList<TileItemData> positions) {
		if(positions != null) {	
			foreach(TileItemData item in positions) {
				Vector2 pos = new Vector2(item.X, item.Y);
				if(!enevlopReplacePositions.ContainsKey(pos)) {
					enevlopReplacePositions.Add(pos, null);
				}
			}
		}

		return Count();
	}

	public int Count() {
		return itemCount + enevlopReplacePositions.Count; 
	}

	public int GetItemCount() {
		return itemCount;
	}
}
