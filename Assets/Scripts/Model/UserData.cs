using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserData {

	public IList<string> HeroeIds = new List<string>();

	public void Init() {
		HeroeIds.Add("green");
	}
}
