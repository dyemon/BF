using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSkillScene : WindowScene {

	public GameObjectResources GameObjectResources;
	private IList<HeroSkillData> skills;
	public static string SceneName = "HeroSkill";


	// Use this for initialization
	void Start () {
		skills = (IList<HeroSkillData>)SceneControllerHelper.instance.GetParameter(SceneName);
	}

	public void OnSelectSkill() {
		Close(skills[0]);
	}
}
