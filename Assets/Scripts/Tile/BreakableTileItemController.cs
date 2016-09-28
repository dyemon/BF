using UnityEngine;
using System.Collections;

public class BreakableTileItemController : TileItemController {
	public int health = 1;

	override public int Damage(int damage) {
		return health -= damage;
	}
}
