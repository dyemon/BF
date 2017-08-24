using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Common.Net.Http {
	
public class HttpResponse {
	string response;
	string error;

	IDictionary<string, string> parameters = new Dictionary<string, string>();
	public IDictionary<string, string> Parameters {
		get { return parameters; }
	}
	public string GetParameter(string name) {
		string val = null;
		parameters.TryGetValue(name, out val);
		return val;
	}

	public HttpResponse(string response, string error) {
		this.response = response;
		this.error = error;

		if(string.IsNullOrEmpty(error)) {
			ParseParameters();

		}
	}

	public string getAsString() {
		return response;
	}
		
		
	public T GetData<T> () {
		string data = GetParameter("data");
		return JsonUtility.FromJson<T>(data);
	}

	public void AddError(string str) {
		if(string.IsNullOrEmpty(str)) {
			return;
		}
		if(error == null) {
			error = "";
		}

		error += " " +str;
	}

	void ParseParameters() {
		if(string.IsNullOrEmpty(response)) {
			return;
		}


		JSONNode root = JSON.Parse(response);
		if(root == null) {
			AddError("Invalid json format");
			return;
		}

		try {
			foreach (KeyValuePair<string, JSONNode> item in root.AsObject) {
				parameters.Add(item.Key, item.Value.Value);
			}
		} catch(System.Exception) {
			AddError("Invalid json format");
			return;
		}

		if(parameters.ContainsKey("status") && parameters["status"] == "error") {
			if(parameters.ContainsKey("data")) {
				AddError(parameters["data"]);
			} else {
				AddError("Response status is error");
			}
		}
			
	}

	public bool IsError() {
		return !string.IsNullOrEmpty(error);
	}

	public string GetError() {
		return error;
	}

	public bool IsUserNotFound() {
		string data = GetParameter("data");
		if(string.IsNullOrEmpty(data)) {
			return false;
		}

		return  data == "{\"error\":\"user not found\"}";
	}
}
}
