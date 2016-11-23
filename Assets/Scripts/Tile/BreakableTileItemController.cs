using UnityEngine;
using System.Collections;

public class BreakableTileItemController : TileItemController {
	private int health = 1;
	public Sprite[] StateSprites;

	protected override void Start() {
		base.Start();
		health = StateSprites == null? 1 : StateSprites.Length + 1;
	}

	override public int Damage(int damage) {
		health -= damage;

		if(health > 0) {
			SpriteRenderer renderer = GetComponent<SpriteRenderer>();
			renderer.sprite = StateSprites[health - 1];
		}

		return health;
	}
}
