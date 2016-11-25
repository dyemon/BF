using UnityEngine;
using System.Collections;

public class Slime {
	private int ratio;
	private int currentRatio;
	private bool collect = true;

	public Slime(int ratio) {
		this.ratio = ratio;
		currentRatio = 0;
	}

	public void Collect() {
		collect = true;
		currentRatio = 0;
	}

	public TileItemType? DropSlime() {
		if(ratio == 0) {
			return null;
		}

		if(collect) {
			collect = false;
			return null;
		}

		currentRatio++;
		if(currentRatio == ratio) {
			currentRatio = 0;
			return TileItemType.Slime;
		}

		return null;
	}
}
