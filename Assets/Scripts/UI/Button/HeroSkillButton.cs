using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HeroSkillButton : BaseBuyButton<HeroSkillData> {

	public void Init(HeroSkillData skill, OnClickListener<HeroSkillData> onClickListener) {
		Sprite bg = GameObjectResources.GetHeroSkillButtonBg(skill.Type);
		Sprite icon = GameObjectResources.GetHeroSkillIcon(skill.Type);

		base.Init(bg, icon, skill.Name, skill.PricaType, skill.PriceValue, skill, onClickListener);

		Text text = transform.Find("Bg/Description").GetComponent<Text>();
		text.text = skill.Description;

		Image img = transform.Find("Result Text").GetComponent<Image>();
		text = img.transform.Find("Text").GetComponent<Text>();
		if(string.IsNullOrEmpty(skill.ResultText)) {
			img.enabled = false;
			text.enabled = false;
		} else {
			text.text = skill.ResultText;
		}
	}

	override protected Button GetButton() {
		return transform.Find("Bg").GetComponent<Button>();;
	}
}
