using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero {
	class HeroItemRatio {
		public TileItemType type;
		public int ratio;
	}

	private HeroData heroData;
	private IList<HeroItemRatio> ratios = new List<HeroItemRatio>();

	public Hero(HeroData hd) {
		heroData = hd;

		for(int i = 0;i < heroData.HeroItemRatio.Length;i++ ) {
			if(heroData.HeroItemRatio[i] > 0) {
				HeroItemRatio r = new HeroItemRatio();
				r.ratio = heroData.HeroItemRatio[i];
				r.type = heroData.Type;
				ratios.Add(r);
			}
		}
	}

	public TileItemData GetHeroItemData() {
		if(ratios.Count == 0) {
			return null;
		}

		HeroItemRatio ratio = ratios[Random.Range(0, ratios.Count)];

		if(Random.Range(0, ratio.ratio) > 0) {
			return null;
		}

		TileItemData item = new TileItemData(0, 10, ratio.type );
		item.Level = GetSkilLevel();
		return item;
	} 

	public int GetSkilLevel() {
		return (heroData.Level > 0)? (int)heroData.Level/5 + 1 : 0;
	}
}
