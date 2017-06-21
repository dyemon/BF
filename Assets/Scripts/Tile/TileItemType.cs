using UnityEngine;
using System.Collections;

public enum TileItemTypeGroup {
	Red = 0, Green = 100, Blue = 200, Yellow = 300, Purple = 400,
	Bomb = 500, Static = 600, Box = 700, SpecialStatic = 800, Special = 900
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
	StaticSlime_2 = TileItemTypeGroup.Static + 5,
	GeneratorBlue = TileItemTypeGroup.Static + 6,

	Box_1 = TileItemTypeGroup.Box,

	Slime = TileItemTypeGroup.SpecialStatic,
	EnemySkillEater = TileItemTypeGroup.SpecialStatic + 1,
		


	BombH = TileItemTypeGroup.Bomb,
	BombV = TileItemTypeGroup.Bomb + 1,
	BombP = TileItemTypeGroup.Bomb + 2,
	BombC = TileItemTypeGroup.Bomb + 3,

	Mobile = TileItemTypeGroup.Special,
	Ring = TileItemTypeGroup.Special + 1,
	Star = TileItemTypeGroup.Special + 2,

	BombAll = TileItemTypeGroup.Special + TileItem.BOMBALL_OFFSET,
	EnemySkillSimple = TileItemTypeGroup.Special + TileItem.ENEMY_SKILL


}
