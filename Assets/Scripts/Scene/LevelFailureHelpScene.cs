using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailureHelpScene : WindowScene {
	public static string SceneName = "LevelFailureHelp";

	public GameObjectResources GameObjectResources;

	public Text Title;
	public Text Description;
	public Image HelpType;
	public Text HelpEffect;
	public BuyButton HelpButton;

	public Sprite[] HelpIcon; 
	private string[] titleText = {"Кончилось здоровье!","Кончились ходы!" }; 
	private string[] descriptionText = {"Восстанови треть жизни и продолжай стрелку!", "Восстанови {0} ходов и продолжай стрелку!"};
	private string[] helpButtonText = {"Лечение", "Добавить"};

	private LevelFailureType reason = LevelFailureType.HealthEnded;
	private HeroController heroController;
	private RestrictionsController restrictionsController;

	private LevelFailureHelpData helpData;

	void Start () {
		if(SceneControllerHelper.instance != null) {
			System.Object[] param = (System.Object[])SceneControllerHelper.instance.GetParameter(SceneName);
			reason = (LevelFailureType)param[0];
			heroController = (HeroController)param[1];
			restrictionsController = (RestrictionsController)param[2];
		}

		helpData = GameResources.Instance.GetGameData().LevelFailureHelpData;

		Title.text = titleText[(int)reason];
		string descr = (reason == LevelFailureType.HealthEnded) ? descriptionText[(int)reason] : string.Format(descriptionText[(int)reason], helpData.Turns);
		Description.text = descr;
		HelpType.sprite = HelpIcon[(int)reason];

	

		HelpButton.Init(helpData.PriceType, helpData.PriceValue, helpButtonText[(int)reason]);

		string effect;
		if(reason == LevelFailureType.TurnsEnded) {
			effect = "+" + helpData.Turns;
		} else {
			effect = "+" + Mathf.Round((float)heroController.StartHealth / helpData.Health ).ToString();
		}
		HelpEffect.text = effect;
	}

	public void OnAcceptHelp() {
		if(!GameResources.Instance.ChangeUserAsset(helpData.PriceType, -helpData.PriceValue)) {
			SceneController.Instance.ShowUserAssetsScene(helpData.PriceType, true);
			return;
		}

		if(reason == LevelFailureType.HealthEnded) {
			heroController.IncreaseHealth((int)Mathf.Round((float)heroController.StartHealth / helpData.Health), false);
		} else {
			restrictionsController.IncreaseCurrentTurns(helpData.Turns);
		}

		GameResources.Instance.SaveUserData(null, false);
		Close();
	}
}
