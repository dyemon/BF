using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/GameObjectResources")]
public class GameObjectResources : ScriptableObject {
	public Sprite[] UserAssetsIcone;
	public Sprite[] HeroSkillIcon;
	public Sprite[] HeroSkillButtonBg;
	public Sprite ExperienceIcon;
	public Sprite[] IconBackground;

	[System.Serializable]
	public class TargetIcon {
		public TargetType Type;
		public Sprite Icon;
	}

	public TargetIcon[] TargetIcons;


	public Sprite GetUserAssetIcone(UserAssetType type) {
		return UserAssetsIcone[(int)type];
	}

	public Sprite GetHeroSkillIcon(HeroSkillType type) {
		return HeroSkillIcon[(int)type];
	}

	public Sprite GetHeroSkillButtonBg(HeroSkillType type) {
		return HeroSkillButtonBg[GetHeroSkillButtonBgIndex(type)];
	}

	int GetHeroSkillButtonBgIndex(HeroSkillType type) {
		if(HeroSkillData.DamageEffects.Contains(type) ||
		    HeroSkillData.StunEffects.Contains(type) ||
			HeroSkillData.SlowdownEffects.Contains(type)) {
			return 0;
		} else if(HeroSkillData.HealthEffects.Contains(type) ||
		          HeroSkillData.EnergyEffects.Contains(type) ||
			type == HeroSkillType.Invulnerability) {
			return 1;
		}

		return 2;
	}

	public Sprite GetTargetIcon(TargetType type) {
		foreach(TargetIcon icon in TargetIcons) {
			if(type == icon.Type) {
				return icon.Icon;
			}
		}
		return null;
	}

	public Sprite GetUserExperienceIcone() {
		return ExperienceIcon;
	}

	public Sprite GetIconBackground(UserAssetType type) {
		switch(type) {
		case UserAssetType.Money:
			return IconBackground[0];
		case UserAssetType.Energy:
			return IconBackground[4];
		case UserAssetType.Ring:
			return IconBackground[8];
		case UserAssetType.Mobile:
			return IconBackground[7];
		case UserAssetType.Star:
			return IconBackground[6];
		default:
			throw new System.Exception("Can not detect background icon for" + type);
		}
	}
}
