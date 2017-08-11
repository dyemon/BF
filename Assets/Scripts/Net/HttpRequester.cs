using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Text;
using System.Collections.Generic;

public class HttpRequester : MonoBehaviour {
	public static string URL_USER_LOAD = "/user/load";
	public static string URL_USER_SAVE = "/user/save";
	public static string URL_SEND_GIFT = "/send/gift";

	public static HttpRequester Instance;
	private string baseUrl = null;

	void Awake() {
		Instance = this;
	}

	public void SetBaseUrl(string url) {
		baseUrl = url;
	}

	private IEnumerator SendInternal(HttpRequest request) {
		Preconditions.Check(this.baseUrl != null || request.getBaseUrl() != null , "Base url can not be null");
		string baseUrl = (request.getBaseUrl() == null) ? this.baseUrl : request.getBaseUrl();
		string url = string.Concat(baseUrl, request.getUrl(), "?", request.GetParamsString());
		Debug.Log("Send request " + url);

		Dictionary<string, string> headers = new Dictionary<string, string>();
	//	Hashtable headers = new Hashtable();

		WWW www = null;

		if(request.getPostData() != null) {
			headers.Add("Content-Type", "application/json");

			www = new WWW(url, Encoding.UTF8.GetBytes(request.getPostData()), headers );
		} else {
			www = new WWW(url);
		} 

		 

		ModalPanel panel = null;
		if(request.IsShowWaitPanel()) {
			panel = ModalPanels.Show(ModalPanelName.WaitPanel);
		}

		while(!www.isDone) {
			yield return null;
		}

		if(panel != null) {
			panel.ClosePanel();
		}
	
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.LogError("[Network] SendRequest ERROR:" + www.error);
			if(request.IsShowErrorMessage() || App.IsShowHttpError) {
				ModalPanels.Show(ModalPanelName.ErrorPanel, www.error);
			}
			if(request.getError() != null) {
				request.getError()(new HttpResponse(www.text, www.error));
			}
		} else {
			if(request.getSuccess() != null) {
				request.getSuccess()(new HttpResponse(www.text, www.error));
			}
		}
	}

	public void Send(string url, HttpRequest.OnSuccess onSuccess, HttpRequest.OnError onError) {
		HttpRequest request = new HttpRequest().Url(url).Success(onSuccess).Error(onError);
		Send(request);
	}

	public void Send(string url, HttpRequest.OnSuccess onSuccess) {
		HttpRequest request = new HttpRequest().Url(url).Success(onSuccess);
		Send(request);
	}

	public void Send(HttpRequest request) {
		StartCoroutine(SendInternal(request));
	}
	

}
