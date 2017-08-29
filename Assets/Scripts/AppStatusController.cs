using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class AppStatusController : MonoBehaviour {
	private bool start = true;

	// Use this for initialization
	void Start () {
		
	}
	
	void OnApplicationPause (bool pauseStatus) {
		UserData uData = GameResources.Instance.GetUserData();

		Debug.Log("OnApplicationPause " + pauseStatus);

		if(start) {
			start = false;
			return;
		}

		if(!pauseStatus) {
			uData.InitTimestampOnStart();
			GameTimers.Instance.Init(uData);
			GameResources.Instance.SaveUserData(uData, true);
			ActivateFB();
		} else {
			GameResources.Instance.SaveUserData(uData, false);
			GameTimers.Instance.Stop();
		}
	}

	void ActivateFB() {
		//app resume
		if (FB.IsInitialized) {
			FB.ActivateApp();
		} 
	}

	void OnApplicationQuit() {
		
	}
}
