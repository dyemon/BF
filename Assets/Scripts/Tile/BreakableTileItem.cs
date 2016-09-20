using UnityEngine;
using System.Collections;

public class BreakableTileItem : TileItem {
	private int health;

	public BreakableTileItem(TileItemType type, GameObject go, int health) : base(type, go){
		this.health = health;
	}

	override public int Damage(int damage) {
		return health -= damage;
	}
}
