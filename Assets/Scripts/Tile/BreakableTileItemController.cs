using UnityEngine;
using System.Collections;
using System;

public class BreakableTileItemController : TileItemController {
	private int health = 1;
	public Sprite[] StateSprites;
	public Sprite brokeSprite;
	private bool broke = false;
	private int startHealth = 0;

	protected override void Start() {
		base.Start();
		health = StateSprites == null? 1 : StateSprites.Length + 1;
		if(startHealth > 0) {
			SetStartHealth(startHealth);
		}
	}

	override public int Damage(int damage) {
		health -= damage;

		if(health > 0) {
			render.sprite = StateSprites[health - 1];
		} else if(brokeSprite != null && !broke) {
			render.sprite = brokeSprite;
			broke = true;
		}

		return health;
	}

	private int getMaxHealth() {
		return StateSprites == null ? 1 : StateSprites.Length + 1;
	}

	override public bool SetStartHealth(int health) {
		if(getMaxHealth() < health ) {
			return false;
		}

		startHealth = health;

		if(render == null) {
			return true;
		}

		this.health = health;
		if(health < getMaxHealth()) {
			render.sprite = StateSprites[health - 1];
		}
		startHealth = 0;

		return true;
	}

	override public bool DestroyOnBreak() {
		return brokeSprite == null;
	}
}
