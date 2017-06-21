using UnityEngine;
using System.Collections;

public enum TargetType {
	Red = TileItemTypeGroup.Red,
	Green = TileItemTypeGroup.Green,
	Blue = TileItemTypeGroup.Blue,
	Yellow = TileItemTypeGroup.Yellow,
	Purple = TileItemTypeGroup.Purple,
	BombAll = TileItemType.BombAll,
	Box = TileItemTypeGroup.Box,
	Enemy = TileItemTypeGroup.Special + 1000
}
