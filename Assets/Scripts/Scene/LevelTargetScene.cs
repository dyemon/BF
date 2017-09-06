using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTargetScene : WindowScene {
	public const string SceneName = "LevelTarget";

	public GameObjectResources GOResources;
	public Text Description;
	public HorizontalLayoutGroup TargetList;
	public GameObject TargetIten;
	public GameObject StartButton;

	protected override void Start() {
		base.Start();

		int level = App.CurrentLevel;
		LevelData levelData = GameResources.Instance.GetLevel(level);

		Description.text = level + ": " + levelData.Name;

		foreach(TargetData item in levelData.TargetData) {
			GameObject ti = Instantiate(TargetIten, TargetList.transform);
			Image img = ti.transform.Find("Image").GetComponent<Image>();
			img.sprite = GOResources.GetTargetIcon(item.Type);
			Text text = ti.transform.Find("Text").GetComponent<Text>();
			text.text = item.Count.ToString();
		}

		StartButton.GetComponent<BuyButton>().Init(UserAssetType.Energy, levelData.LevelPrice, null);

	}
	

}
