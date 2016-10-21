using UnityEngine;
using System.Collections;

public interface IFBCallback {
	void OnFBInit();
	void OnFBLoginSuccess();
	void OnFBLogout();
	void OnFBLoginFail(string error);
}
