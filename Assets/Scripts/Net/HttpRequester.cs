using UnityEngine;
using System.Collections;
using Common.Net.Http;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class HttpRequester : MonoBehaviour {
	public static string URL_USER_LOAD = "/chotkiy/userGet";
	public static string URL_USER_SAVE = "/chotkiy/userSave";
	public static string URL_SEND_GIFT = "/chotkiy/sendGift";
	public static string URL_CHECK_GIFT = "/chotkiy/checkGifts";

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

		Dictionary<string, string> headers = new Dictionary<string, string>();

		UnityWebRequest www = null;

		if(request.GetHttpMethod() == "POST") {
			WWWForm form = new WWWForm();
			foreach(KeyValuePair<string, string> item in request.RequestParameters) {
				form.AddField(item.Key, item.Value);
			}
			string url = string.Concat(baseUrl, request.GetUrl());
			www = UnityWebRequest.Post(url, form);
		} else {
			www = UnityWebRequest.Get(string.Concat(baseUrl, request.GetUrl(), "?", request.GetQueryString()));
		}
			
		ModalPanel panel = null;
		if(request.IsShowWaitPanel()) {
			panel = ModalPanels.Show(ModalPanelName.WaitPanel);
		}

	//	while(!www.isDone) {
	//		yield return null;
	//	}
		yield return www.Send();

		if(panel != null) {
			panel.ClosePanel();
		}
	
		HttpResponse response = new HttpResponse(www.downloadHandler.text, www.error);
		if(string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.downloadHandler.text)) {
			CheckSig(response);
		}

	//	Debug.Log(www.downloadHandler.text); 
		string srvtime = response.GetParameter("_srvtime");
		if(!string.IsNullOrEmpty(srvtime)) {
			try {
				long stime = long.Parse(srvtime);
				LocalData lData = GameResources.Instance.GetLocalData();
				lData.SrvTime = stime;
				GameResources.Instance.SaveLocalData();
			} catch (  System.Exception) {
			}
		}

		if (response.IsError()) {
			Debug.LogError("[Network] SendRequest ERROR:" + response.GetError());
			if(request.IsShowErrorMessage() || App.IsShowHttpError) {
				ModalPanels.Show(ModalPanelName.ErrorPanel, response.GetError());
			}
			FireError(request.GetUrl(), response);
		} else {
			Debug.Log(www.downloadHandler.text);

			FireSuccess(request.GetUrl(), response);
		}
	}
		
	public void Send(HttpRequest request, bool addAuthParams = true) {
		if(addAuthParams) {
			AddAuthParams(request);
		}
		Debug.Log("Send http " + request.GetUrl() + " Data: " + request.GetQueryString());

	//	StartCoroutine(SendInternal(request));
	}

	void AddAuthParams(HttpRequest request) {
		request.Param("socialId", Account.Instance.GetUserId())
			.Param("socialType", Account.Instance.SocialType);

		string str = "";
		foreach(KeyValuePair<string, string> item in request.RequestParameters.OrderBy(key => key.Key)) {
			str += item.Key + item.Value;
		}
		str += Salt;

	//	Debug.Log("**** " + str);
		request.Param("_sig", MD5.Hash(str));
	}
		
	void CheckSig(HttpResponse response) {
		string str = "";
		string sig = null;
		foreach(KeyValuePair<string, string> item in response.Parameters.OrderBy(key => key.Key)) {
			if(item.Key == "_sig") {
				sig = item.Value;
			} else {
				str += item.Key + item.Value;
			}

		}
		str += Salt;

		if(string.IsNullOrEmpty(sig)) {
			response.AddError(" Sig is empty");
			return;
		}
		string hash = MD5.Hash(str);
	//	Debug.Log("CheckSign **** " + str + " Sig: " +sig+ " Hash: " +hash);

		if(hash != sig) {
			response.AddError("Invalid sig");
		}
	}

	string Salt {
		get { return "dhy237dhwydb203mandpnsDeb5238dbo"; }
	}

	private IDictionary<string, System.Delegate> onSuccessEvents = new Dictionary<string, System.Delegate>();
	private IDictionary<string, System.Delegate> onErrorEvents = new Dictionary<string, System.Delegate>();

	public void AddEventListener(string url, HttpRequest.OnSuccess onSuccess, HttpRequest.OnError onError = null) {
		if(onSuccess != null) {
			System.Delegate old;
			onSuccessEvents.TryGetValue(url, out old);
			if(old == null) {
				onSuccessEvents[url] = onSuccess;
			} else {
				onSuccessEvents[url] = System.Delegate.Combine(old, onSuccess);
			}
		}
		if(onError != null) {
			System.Delegate old;
			onErrorEvents.TryGetValue(url, out old);
			if(old == null) {
				onErrorEvents[url] = onError;
			} else {
				onErrorEvents[url] = System.Delegate.Combine(old, onError);
			}
		}
	}

	public void RemoveEventListener(string url, HttpRequest.OnSuccess onSuccess, HttpRequest.OnError onError = null) {
		if(onSuccess != null) {
			System.Delegate old;
			onSuccessEvents.TryGetValue(url, out old);
			if(old != null) {
				System.Delegate currentDel = System.Delegate.Remove(old, onSuccess);
				if (currentDel == null){
					onSuccessEvents.Remove(url);
				}
				else{
					onSuccessEvents[url] = currentDel;
				}
			} 
		}
		if(onError != null) {
			System.Delegate old;
			onErrorEvents.TryGetValue(url, out old);
			if(old != null) {
				System.Delegate currentDel = System.Delegate.Remove(old, onError);
				if (currentDel == null){
					onErrorEvents.Remove(url);
				}
				else{
					onErrorEvents[url] = currentDel;
				}
			} 
		}
	}

	public void FireSuccess(string url, HttpResponse response) {
		System.Delegate d;
		onSuccessEvents.TryGetValue(url, out d);
		if(d != null) {
			d.DynamicInvoke(response);
		}
	}

	public void FireError(string url, HttpResponse response) {
		System.Delegate d;
		onErrorEvents.TryGetValue(url, out d);
		if(d != null) {
			d.DynamicInvoke(response);
		}
	}
}
