using UnityEngine;
using System.Collections;

public class HeroData  {

	public TileItemTypeGroup TypeGroup;
	public int[] HeroItemRatio;

	public HeroData(TileItemTypeGroup typeGroup, int bombRatio, int envelopRatio) {
		this.TypeGroup = typeGroup;
		HeroItemRatio = new int[2];
		HeroItemRatio[0] = bombRatio;
		HeroItemRatio[1] = envelopRatio;
	}
}
