using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSuccessScene : MonoBehaviour {
	public GameObject AwardItem;
	public HorizontalLayoutGroup Awards1;
	public HorizontalLayoutGroup Awards2;
	public GameObject Experience;

	private int successCount = 1;

	void OnDisable() {
		GameResources.Instance.SaveUserData(null, successCount == 1);
	}

	void Start () {
		int i = 0;
		LevelAwardData awardData = GameResources.Instance.GetLevel(App.GetCurrentLevel()).SuccessAwardData;
		UserData uData = GameResources.Instance.GetUserData();
		successCount = 1;//uData.GetSuccessCount(App.GetCurrentLevel()); 
		Preconditions.Check(successCount > 0, "Success level count for level {0} = 0", App.GetCurrentLevel());

		foreach(UserAssetType type in EnumUtill.GetValues<UserAssetType>()) {
			int collectVal = GameController.CollectLevelAward.GetAsset(type);
			int awardVal = awardData.GetAsset(type);

			if(collectVal > 0 || awardVal > 0) {
				HorizontalLayoutGroup layout = (i++ > 1) ? Awards2 : Awards1;
				GameObject go = Instantiate(AwardItem, layout.transform);
				go.GetComponent<BuyButton>().Init(type, collectVal, null);
			}

			if(awardVal > 0) {
				int award = (int)Mathf.Floor(awardVal / Mathf.Pow(2, successCount - 1));
				if(award > 0) {
					GameResources.Instance.ChangeUserAsset(uData, type, award);
				}
			}
		}
			
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards1.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(Awards2.GetComponent<RectTransform>());

		Experience.GetComponent<BuyButton>().Init(null, GameController.CollectLevelAward.Experience, null, UserAssetTypeExtension.ExperienceColor);
	}
	

}
