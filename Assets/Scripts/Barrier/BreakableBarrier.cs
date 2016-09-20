using UnityEngine;
using System.Collections;

public class BreakableBarrier : Barrier {

	private int health;

	public BreakableBarrier(BarrierData data, GameObject go, int health) : base(data, go){
		this.health = health;
	}

	override public int Damage(int damage) {
		return health -= damage;
	}
}
