using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/GameObjectResources")]
public class GameObjectResources : ScriptableObject {
	public Sprite[] UserAssetsIcone;
	public Sprite[] HeroSkillIcon;

	public Sprite GetUserAssetIcone(UserAssetType type) {
		return UserAssetsIcone[(int)type];
	}

	public Sprite GetHeroSkillIcon(HeroSkillType type) {
		return HeroSkillIcon[(int)type];
	}
}
