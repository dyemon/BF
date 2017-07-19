using UnityEngine;
using System.Collections;

public class MapScene : BaseScene {
	public const string SceneName = "Map";

	void Start () {
		Invoke("ShowAdditionScenes", 2);
	}
	
	public void OnClickFB() {
		if(!Account.Instance.IsLogged) {
			SceneController.Instance.LoadSceneAdditive("FBNotLogged");
		} else {
			SceneController.Instance.LoadSceneAdditive("FBLogged");
		}
	}


}
