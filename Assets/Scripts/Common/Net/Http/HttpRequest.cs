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
		private OnSuccess onSuccess = null;
		private OnError onError = null;
		private bool showWaitPanel = false;
		private bool showErrorMessage = false;

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

		public HttpRequest Url(string url) {
			this.url = url;
			return this;
		}
		public string getUrl() {
			return url;
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

		public HttpRequest Success(OnSuccess onSuccess) {
			this.onSuccess = onSuccess;
			return this;
		}
		public OnSuccess getSuccess() {
			return onSuccess;
		}

		public HttpRequest Error(OnError onError) {
			this.onError = onError;
			return this;
		}
		public OnError getError() {
			return onError;
		}

		public string GetParamsString() {
			return string.Join("&", requestParams.Select(n => WWW.EscapeURL(n.Key) + "=" + WWW.EscapeURL(n.Value)).ToArray());
		}
	

	
	}
}