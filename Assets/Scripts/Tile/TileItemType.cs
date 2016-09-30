using UnityEngine;
using System.Collections;

public enum TileItemTypeGroup {
	Red = 0, Green = 20, Blue = 40, Yellow = 60, Purple = 80,
	Static = 400, Special = 500
}

[System.Serializable]
public enum TileItemType {		
	Red = TileItemTypeGroup.Red,
	Green = TileItemTypeGroup.Green, 
	Blue = TileItemTypeGroup.Blue, 
	Yellow = TileItemTypeGroup.Yellow, 
	Purple = TileItemTypeGroup.Purple,

	RedBombH = TileItemTypeGroup.Red + TileItem.BOMBH_OFFSET,
	GreenBombH = TileItemTypeGroup.Green + TileItem.BOMBH_OFFSET, 
	BlueBombH = TileItemTypeGroup.Blue + TileItem.BOMBH_OFFSET, 
	YellowBombH = TileItemTypeGroup.Yellow + TileItem.BOMBH_OFFSET, 
	PurpleBombH = TileItemTypeGroup.Purple + TileItem.BOMBH_OFFSET,

	RedBombV = TileItemTypeGroup.Red + TileItem.BOMBV_OFFSET,
	GreenBombV = TileItemTypeGroup.Green + TileItem.BOMBV_OFFSET, 
	BlueBombV = TileItemTypeGroup.Blue + TileItem.BOMBV_OFFSET, 
	YellowBombV = TileItemTypeGroup.Yellow + TileItem.BOMBV_OFFSET, 
	PurpleBombV = TileItemTypeGroup.Purple + TileItem.BOMBV_OFFSET,

	RedEnvelop = TileItemTypeGroup.Red + TileItem.ENVELOP_OFFSET,
	GreenEnvelop = TileItemTypeGroup.Green + TileItem.ENVELOP_OFFSET, 
	BlueEnvelop = TileItemTypeGroup.Blue + TileItem.ENVELOP_OFFSET, 
	YellowEnvelop = TileItemTypeGroup.Yellow + TileItem.ENVELOP_OFFSET, 
	PurpleEnvelop = TileItemTypeGroup.Purple + TileItem.ENVELOP_OFFSET,

	Static_1 = TileItemTypeGroup.Static,
	Static_2 = TileItemTypeGroup.Static + 1,

	Brilliant = TileItemTypeGroup.Special + TileItem.BRILLIANT_OFFSET
		
}
