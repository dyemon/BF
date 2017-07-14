using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlathataScene : WindowScene {
	public GameObjectResources GOResources;
	public GameObject[] awardBoxes;
	public UserAssetsPanel AssetsPanel;
	public GameObject AwardItem;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, false);
	}

	void Start () {
		foreach(GameObject item in awardBoxes) {
			item.GetComponent<Button>().onClick
				.AddListener(() => {OnSelectBox(item);});
		}
	}
	
	void OnSelectBox(GameObject box) {
		AwardItem award = new AwardItem();
		award.Value = 5;

		AssetsPanel.DisableUpdate(true);
		GameResources.Instance.ChangeUserAsset(award.Type, award.Value);
		AssetsPanel.DisableUpdate(false);

	
		GameObject animImg = Instantiate(AwardItem, box.transform.position, Quaternion.identity);
		animImg.transform.SetParent(transform);
		animImg.AddComponent<AnimatedObject>();
		Vector3 end = AssetsPanel.GetUserAssetsIcon(award.Type).transform.position;
		Vector3 start = box.transform.position;
		Sprite icon = GOResources.GetUserAssetIcone(award.Type);

		Animations.CreateAwardAnimation(animImg, start, end, icon, award.Value); 
		animImg.GetComponent<AnimatedObject>()
			.OnStop(() => {CompleteTakeBox(animImg, box);} ).Run();

		box.GetComponent<AnimatedObject>().AddFadeUI(null, 0, 1f).Build().Run();
	}

	void CompleteTakeBox(GameObject animGO, GameObject box) {
		Destroy(animGO);
		AssetsPanel.UpdateUserAssets();
		StartCoroutine(FadeInBox(box));
	}

	IEnumerator FadeInBox(GameObject box) {
		yield return new WaitForSeconds(5);
		box.GetComponent<AnimatedObject>().AddFadeUI(null, 1, 1f).Build().Run();
	}
}
