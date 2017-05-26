using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HeroSkillButton : BuyButton {

	public void Init(HeroSkillData skill) {
		Sprite bg = GameObjectResources.GetHeroSkillButtonBg(skill.Type);
		Sprite icon = GameObjectResources.GetHeroSkillIcon(skill.Type);

		base.Init(bg, icon, skill.Name, skill.PricaType, skill.PriceValue);

		Text text = transform.FindChild("Button/Description").GetComponent<Text>();
		text.text = skill.Description;

		Image img = transform.FindChild("Result Text").GetComponent<Image>();
		text = img.transform.FindChild("Text").GetComponent<Text>();
		if(string.IsNullOrEmpty(skill.ResultText)) {
			img.enabled = false;
			text.enabled = false;
		} else {
			text.text = skill.ResultText;
		}
	}


}
