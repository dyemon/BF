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

	RedBomb = TileItemTypeGroup.Red + TileItem.BOMB_OFFSET,
	GreenBomb = TileItemTypeGroup.Green + TileItem.BOMB_OFFSET, 
	BlueBomb = TileItemTypeGroup.Blue + TileItem.BOMB_OFFSET, 
	YellowBomb = TileItemTypeGroup.Yellow + TileItem.BOMB_OFFSET, 
	PurpleBomb = TileItemTypeGroup.Purple + TileItem.BOMB_OFFSET,

	RedEnvelop = TileItemTypeGroup.Red + TileItem.ENVELOP_OFFSET,
	GreenEnvelop = TileItemTypeGroup.Green + TileItem.ENVELOP_OFFSET, 
	BlueEnvelop = TileItemTypeGroup.Blue + TileItem.ENVELOP_OFFSET, 
	YellowEnvelop = TileItemTypeGroup.Yellow + TileItem.ENVELOP_OFFSET, 
	PurpleEnvelop = TileItemTypeGroup.Purple + TileItem.ENVELOP_OFFSET,

	Unavaliable_1 = TileItemTypeGroup.Unavaliable,
	Unavaliable_2 = TileItemTypeGroup.Unavaliable + 1,

	Brilliant = TileItemTypeGroup.Special + TileItem.BRILLIANT_OFFSET
		
}
