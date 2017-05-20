using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HeroSkillController : MonoBehaviour {
	public GameObjectResources GameObjectResources;
	public Image ActiveHeroSkillIcon;

	private IDictionary<HeroSkillType, HeroSkillData> effectiveSkills = new Dictionary<HeroSkillType, HeroSkillData>();

	void Start () {
		
	}
	
	public void AddSkill(HeroSkillData skill) {
		effectiveSkills[skill.Type] = skill;
		skill.RemainTurns = skill.Turns;

		Image icon = FindIcon(skill.Type);
		if(icon == null) {
			icon = (Image)Instantiate(ActiveHeroSkillIcon);
			icon.transform.SetParent(gameObject.transform);
			icon.transform.localScale = new Vector3(1, 1, 1);
			icon.sprite = GameObjectResources.GetHeroSkillIcon(skill.Type);
			icon.name = skill.Type.ToString();

		//	if(skill.Type == HeroSkillType.ExcludeColor) {
		//		icon.color = TileItem.ConvertToColor(skill.ExcludeColor);
	//		}
		}
			
		Text text = icon.transform.Find("Text").gameObject.GetComponent<Text>();
		text.text = skill.RemainTurns.ToString();
	}
		
	public IList<HeroSkillData> GetSkills(HeroSkillType[] types) {
		IList<HeroSkillData> res = new List<HeroSkillData>();

		foreach(HeroSkillType type in types) {
			if(effectiveSkills.ContainsKey(type)) {
				res.Add(effectiveSkills[type]);
			}
		}

		return res;
	}

	public void OnTurnComplete() {
		foreach(Image icon in GetComponentsInChildren<Image>()) {
			if(icon.name == "HeroSkillPanel") {
				continue;
			}
			HeroSkillData skill = effectiveSkills[EnumUtill.Parse<HeroSkillType>(icon.name)];
			if(--skill.RemainTurns == 0) {
				effectiveSkills.Remove(skill.Type);
				Destroy(icon.gameObject);
			} else {
				Text text = icon.transform.Find("Text").gameObject.GetComponent<Text>();
				text.text = skill.RemainTurns.ToString();
			}
		}

	}

	void RemoveSkill(KeyValuePair<HeroSkillType, HeroSkillData> keyVal) {
		effectiveSkills.Remove(keyVal.Key);
	}

	Image FindIcon(HeroSkillType type) {
		Transform tr = gameObject.transform.Find(type.ToString());
		return tr == null? null : tr.gameObject.GetComponent<Image>();
	}

	public float GetPowerPointMultiplier() {
		float mult = 1f;
		foreach(HeroSkillData skill in effectiveSkills.Values) {
			if(skill.Energy > 0) {
				mult *= skill.Energy / 100f; 
			}
		}

		return mult;
	}

	public bool IsInvulnerability() {
		return effectiveSkills.Contains(HeroSkillType.Invulnerability);
	}
}
