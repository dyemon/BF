using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KachalkaData {
	public string TypeAsString;
	public KachalkaType Type;
	public string Name;
	public KachalkaItem[] Items;

	public void Init() {
		Type = EnumUtill.Parse<KachalkaType>(TypeAsString);

		foreach(KachalkaItem item in Items) {
			item.Init();
		}
	}
}
