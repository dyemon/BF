using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserData {

	public IDictionary<string, int> HeroeIds = new Dictionary<string, int>();

	public void Init() {
	//	HeroeIds.Add("blueEnvelop", 3);
		HeroeIds.Add("redHBomb", 1);
	//	HeroeIds.Add("redEnvelop", 1);
		HeroeIds.Add("greenVBomb", 5);

	}
}
