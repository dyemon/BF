using UnityEngine;
using System.Collections;

public enum TileItemTypeGroup {
	Normal = 0, NotAvaliable = 20, NotInteract = 40, Special = 60
}

public enum TileItemType {
	Red = 0, Green = 1, Blue = 2, Yellow = 3, Purple = 4,
	NotAvaliable_1 = TileItemTypeGroup.NotAvaliable
		
}
