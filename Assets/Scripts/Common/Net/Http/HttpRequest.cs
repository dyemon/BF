using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.Net.Http {
	
public class HttpRequest {
	public delegate void OnSuccess (HttpResponse response);
	public delegate void OnError (HttpResponse response);

	private Dictionary<string, string> requestParams = new Dictionary<string, string>();

	private string baseUrl = null;
	private string url = null;
	private bool showWaitPanel = false;
	private bool showErrorMessage = false;
	private string httpMethod = "GET";

	public HttpRequest(string url) {
		this.url = url;
	}

	public HttpRequest BaseUrl(string url) {
		baseUrl = url;
		return this;
	}
	public string getBaseUrl() {
		return baseUrl;
	}

	public HttpRequest Param(string key, string value) {
		if(requestParams.ContainsKey(key)) {
			requestParams[key] = value;
		} else {
			requestParams.Add(key, value);
		}

		return this;
	}

	public HttpRequest Param(string key, long value) {
		return Param(key, value.ToString());
	}

	public HttpRequest Param(string key, bool value) {
		return Param(key, value ? 1 : 0);
	}

	public HttpRequest RemoveParam(string key) {
		if(requestParams.ContainsKey(key)) {
			requestParams.Remove(key);
		}

		return this;
	}

	public HttpRequest HttpMethod(string val) {
		httpMethod = val;
		return this;
	}

	public string GetUrl() {
		return url;
	}
	public string GetHttpMethod() {
		return httpMethod;
	}

	public HttpRequest ShowWaitPanel(bool showWaitPanel) {
		this.showWaitPanel = showWaitPanel;
		return this;
	}
	public bool IsShowWaitPanel() {
		return showWaitPanel;
	}

	public HttpRequest ShowErrorMessage(bool showErrorMessage) {
		this.showErrorMessage = showErrorMessage;
		return this;
	}
	public bool IsShowErrorMessage() {
		return showErrorMessage;
	}


	public string GetQueryString() {
		return string.Join("&", requestParams.Select(n => WWW.EscapeURL(n.Key) + "=" + WWW.EscapeURL(n.Value)).ToArray());
	}
	public string GetFormParams() {
		return string.Join("&", requestParams.Select(n => n.Key + "=" + n.Value).ToArray());
	}

	public Dictionary<string, string> RequestParameters {
		get { return requestParams; }
	}
}
}