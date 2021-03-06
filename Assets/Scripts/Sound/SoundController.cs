﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
	public static SoundController Instance = new SoundController();

	public const int COINS_VOLUME = 3;
	public const int KASSA_VOLUME = 3;
	public const int STAT_VOLUME = 3;
	public const int EXPERIENCE_VOLUME = 3;

	public AudioClip[] TileItemCollect;
	public AudioClip ComboCollect;
	public AudioClip CollectTileItem;
	public AudioClip BombExplosion;
	public AudioClip Coins;
	public AudioClip Cash;
	public AudioClip Eater;
	public AudioClip Stat;
	public AudioClip Experience;
	public AudioClip Kassa;
	public AudioClip Toggle;
	public AudioClip FortunaRotate;
	public AudioClip FaltAction;
	public AudioClip EnamySkill;

	public AudioClip Strike1;
	public AudioClip Strike2;
	public AudioClip Strike3;
	public AudioClip Strike4;
	public AudioClip HeroSkill;

	public AudioClip WindowOpen;
	public AudioClip WindowClose;

	public AudioClip LevelSuccess;
	public AudioClip LevelFailure;

	public AudioClip FortunaJackpot;

	private bool enable;

	private AudioSource source;
	private int prevTileItemCollect = 0;

	void Awake() {
		Instance = this;
		enable = GameResources.Instance.GetLocalData().SoundOn;
	}

	void Start() {
		source = GetComponent<AudioSource>();
	}

	public void Enable(bool val) {
		enable = val;
	}

	public void PlayClip(AudioClip clip, float volume = 1, float delay = 0) {
		if(!enable) {
			return;
		}

	//	source.clip = clip;
	//	source.Play();
		if(delay > 0) {
			StartCoroutine(PlayInternal(clip, volume, delay));
		} else {
			source.PlayOneShot(clip, volume);
		}
	}

	private IEnumerator PlayInternal(AudioClip clip, float volume, float delay) {
		yield return new WaitForSeconds(delay);
		source.PlayOneShot(clip, volume);
	}

	public static void Play(AudioClip clip, float volume = 1, float delay = 0) {
		if(clip != null) {
			Instance.PlayClip(clip, volume, delay);
		}
	}

	public void PlayTileItemCollect(int index) {
		if(TileItemCollect == null) {
			return;
		}
		if(index > TileItemCollect.Length - 1) {
			index = TileItemCollect.Length - 1;
		}

	//	if(prevTileItemCollect == index) {
	//		return;
		//}

		prevTileItemCollect = index;
		PlayClip(TileItemCollect[index], 0.7f);
	}
}
