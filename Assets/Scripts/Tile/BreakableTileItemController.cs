using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BreakableTileItemController : TileItemController {
	public static IList<AudioClip> PlayedAudio = new List<AudioClip>();

	private int health = 1;
	public Sprite[] StateSprites;
	public Sprite BrokeSprite;
	private bool broke = false;
	private int startHealth = 0;
	public GameObject Splinter;
	public int SplintersCount;
	public AudioClip DestroyAudio;

	protected override void Start() {
		base.Start();
		health = StateSprites == null? 1 : StateSprites.Length + 1;
		if(startHealth > 0) {
			SetStartHealth(startHealth);
		}
	}

	override public int Damage(int damage) {
		health -= damage;
		//SpriteRenderer render = GetComponent<SpriteRenderer>();

		if(health > 0) {
			render.sprite = StateSprites[health - 1];
		} else if(BrokeSprite != null && !broke) {
			render.sprite = BrokeSprite;
			broke = true;
		}

		if(Splinter != null) {
			displaySplinters();
		}

		if(DestroyAudio != null && !PlayedAudio.Contains(DestroyAudio)) {
			PlayedAudio.Add(DestroyAudio);
			SoundController.Play(DestroyAudio);
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
		return BrokeSprite == null;
	}

	private void displaySplinters() {
		for(int i = 0; i < SplintersCount; i++) {
			Instantiate(Splinter, transform.position, Quaternion.identity);
		}
	}
}
