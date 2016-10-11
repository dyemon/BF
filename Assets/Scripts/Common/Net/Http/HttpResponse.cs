using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Net.Http {
	
	public class HttpResponse {
		string data;
		string error;

		public HttpResponse(string data, string error) {
			this.data = data;
			this.error = error;
		}

		public bool IsError() {
			return string.IsNullOrEmpty(error);
		}

		public string getAsString() {
			return data;
		}

		public T FromJson<T> () {
			return JsonUtility.FromJson<T>(data);
		}
			
	}
}
