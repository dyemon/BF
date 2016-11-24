using UnityEngine;
using System.Collections;

public enum TileItemTypeGroup {
	Red = 0, Green = 100, Blue = 200, Yellow = 300, Purple = 400,
	Static = 500, Box = 600, SpecialStatic = 700, Special = 800
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

	RedBombHV = TileItemTypeGroup.Red + TileItem.BOMBHV_OFFSET,
	GreenBombHV = TileItemTypeGroup.Green + TileItem.BOMBHV_OFFSET, 
	BlueBombHV = TileItemTypeGroup.Blue + TileItem.BOMBHV_OFFSET, 
	YellowBombHV = TileItemTypeGroup.Yellow + TileItem.BOMBHV_OFFSET, 
	PurpleBombHV = TileItemTypeGroup.Purple + TileItem.BOMBHV_OFFSET,

	Static_1 = TileItemTypeGroup.Static,
	Static_2 = TileItemTypeGroup.Static + 1,
	Static_3 = TileItemTypeGroup.Static + 2,
	Static_4 = TileItemTypeGroup.Static + 3,
	StaticSlime_1 = TileItemTypeGroup.Static + 4,

	Box_1 = TileItemTypeGroup.Box,

	Brilliant = TileItemTypeGroup.Special + TileItem.BRILLIANT_OFFSET,
	Key = TileItemTypeGroup.Special + TileItem.KEY_OFFSET,
	BombAll = TileItemTypeGroup.Special + TileItem.BOMBALL_OFFSET	
}
