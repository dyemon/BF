using UnityEngine;
using System.Collections;

public enum TileItemTypeGroup {
	Red = 0, Green = 20, Blue = 40, Yellow = 60, Purple = 80,
	Unavaliable = 400, Special = 500
}

public enum TileItemType {
	Red = TileItemTypeGroup.Red,
	Green = TileItemTypeGroup.Green, 
	Blue = TileItemTypeGroup.Blue, 
	Yellow = TileItemTypeGroup.Yellow, 
	Purple = TileItemTypeGroup.Purple,

	Unavaliable_1 = TileItemTypeGroup.Unavaliable,
	Unavaliable_2 = TileItemTypeGroup.Unavaliable + 1
		
}
