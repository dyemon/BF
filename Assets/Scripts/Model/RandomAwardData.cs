using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomAwardData {
	public RandomAwardUserAssetItem[] UserAssets;

	public void Init() {
		foreach(RandomAwardUserAssetItem item in UserAssets) {
			item.Init();
		}
	}

	public AwardItem GetAward() {
		int sum = 0;
		foreach(RandomAwardUserAssetItem item in UserAssets) {
			sum += item.Ratio;
		}

		int rand = Random.Range(0, sum);
		int index = 0;
		sum = 0;
		foreach(RandomAwardUserAssetItem item in UserAssets) {
			sum += item.Ratio;
			if(rand < sum) {
				break;
			}
			index++;
		}

		RandomAwardUserAssetItem sel = UserAssets[index];
		AwardItem res = new AwardItem();
		res.Type = sel.Type;
		res.TypeAsString = res.Type.ToString();
		res.Value = Random.Range(sel.Min, sel.Max + 1);

		return res;
	}
}
