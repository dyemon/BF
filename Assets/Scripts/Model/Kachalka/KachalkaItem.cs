using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KachalkaItem {
	public int MinExperience;
	public int Damage;
	public int Health;
	public PriceItem[] Steps;

	public void Init() {
		foreach(PriceItem item in Steps) {
			item.Init();
		}
	}
}
