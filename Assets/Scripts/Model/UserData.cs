using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserData {

	public IDictionary<string, int> HeroeIds = new Dictionary<string, int>();

	public void Init() {
		HeroeIds.Add("redVBomb", 2);
		HeroeIds.Add("redHBomb", 1);
		HeroeIds.Add("redEnvelop", 2);

		HeroeIds.Add("greenVBomb", 1);
		HeroeIds.Add("greenHBomb", 2);
		HeroeIds.Add("greenEnvelop", 2);

		HeroeIds.Add("blueVBomb", 1);
		HeroeIds.Add("blueHBomb", 1);
		HeroeIds.Add("blueEnvelop", 2);

		HeroeIds.Add("yellowVBomb", 1);
		HeroeIds.Add("yellowHBomb", 1);
		HeroeIds.Add("yellowEnvelop", 2);

		HeroeIds.Add("purpleVBomb", 1);
		HeroeIds.Add("purpleHBomb", 1);
		HeroeIds.Add("purpleEnvelop", 2);
	}
}
