using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroSkillTypeGroup {
	DropTileItem = 0, TileItem = 100, Hero = 200, HeroHealt = 300, HeroDamage = 400, 
	Enemy = 500, EnemyHealt = 600, EnemyDamage = 700
}

public enum HeroSkillType {
	BombV = HeroSkillTypeGroup.DropTileItem,
	BombH = HeroSkillTypeGroup.DropTileItem + 1,
	BombP = HeroSkillTypeGroup.DropTileItem + 2,
	BombC = HeroSkillTypeGroup.DropTileItem + 3,
}
