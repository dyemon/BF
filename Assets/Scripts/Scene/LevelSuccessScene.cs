using UnityEngine;
using System.Collections;

public class LevelSuccessScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameResources.Instance.SaveUserData(null, true);
	}
	

}
