using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSkillScene : WindowScene {

	public GameObjectResources GameObjectResources;
	private IList<HeroSkillData> skills;
	public static string SceneName = "HeroSkill";

	public RectTransform SkillsPanel;
	public GameObject HeroSkillButton;
	// Use this for initialization
	void Start () {
		skills = (IList<HeroSkillData>)SceneControllerHelper.instance.GetParameter(SceneName);
/*		HeroSkillData[] aSkills = GameResources.Instance.GetGameData().HeroSkillData; 
		skills = new List<HeroSkillData>();
		skills.Add(aSkills[11]);
		skills.Add(aSkills[12]);
		skills.Add(aSkills[10]);
*/
		foreach(HeroSkillData skill in skills) {
			GameObject button = Instantiate(HeroSkillButton);
			button.transform.SetParent(SkillsPanel.transform);
			button.transform.localScale = new Vector3(1, 1, 1);
			button.GetComponent<HeroSkillButton>().Init(skill, OnSelectSkill);
		}
	}

	public void OnSelectSkill(HeroSkillData skill) {
		if(!GameResources.Instance.Buy(skill)) {
			return;
		}

		Close(skill);
	}
}
