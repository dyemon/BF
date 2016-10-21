using UnityEngine;
using System.Collections;

public class MapScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnClickFB() {
		if(!Account.Instance.IsLogged) {
			SceneController.Instance.LoadSceneAdditive("FBNotLogged");
		} else {
			SceneController.Instance.LoadSceneAdditive("FBLogged");
		}
	}
}
