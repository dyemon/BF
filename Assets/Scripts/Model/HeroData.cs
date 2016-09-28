using UnityEngine;
using System.Collections;

public class HeroData  {

	public TileItemType Type;
	public int[] HeroItemRatio;
	public int Level;

	public HeroData(TileItemType type, int ration) {
		this.Type = type;
		HeroItemRatio = new int[1];
		HeroItemRatio[0] = ration;
		Level = 0;
	}
}
