using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.Net.Http {
	
	public class HttpRequest : MonoBehaviour {
		public delegate void OnSuccess (HttpResponse response);
		public delegate void OnError (HttpResponse response);

		private Dictionary<string, string> requestParams = new Dictionary<string, string>();
		private static string baseUrl;
		private string url;
		private OnSuccess onSuccess;
		private OnError onError;

		public static void BaseUrl(string url) {
			baseUrl = url;
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

		public HttpRequest Success(OnSuccess onSuccess) {
			this.onSuccess = onSuccess;
			return this;
		}

		public HttpRequest Error(OnError onError) {
			this.onError = onError;
			return this;
		}

		public string GetParamsString() {
			return string.Join("&", requestParams.Select(n => n.Key + "=" + n.Value).ToArray());
		}
	

		private IEnumerator SendInternal() {
			WWW www = new WWW(WWW.EscapeURL(string.Concat(baseUrl, url, "?", GetParamsString())));

			while(!www.isDone) {
				yield return null;
			}

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError("[Network] SendRequest ERROR:" + www.error);
				if(onError != null) {
					onError(new HttpResponse(www.text, www.error));
				}
			} else {
				if(onSuccess != null) {
					onSuccess(new HttpResponse(www.text, www.error));
				}
			}
		}

		public void Send(string url, OnSuccess onSuccess, OnError onError) {
			this.Success(onSuccess).Error(onError).Url(url).Send();
		}

		public void Send(string url, OnSuccess onSuccess) {
			this.Success(onSuccess).Url(url).Send();
		}

		public void Send() {
			StartCoroutine(SendInternal());
		}
	}
}