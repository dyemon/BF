﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersController : MonoBehaviour {
	public static ParametersController Instance;
	private IDictionary<string, System.Object> param = new  Dictionary<string, System.Object>();

	public const string CAPITULATE_NOT_ENDED = "CAPITULATE_NOT_ENDED";

	void Awake() {
		Instance = this;
	}

	public void SetParameter(string key, System.Object val) {
		param[key] = val;
	}
	public void RemoveParam(string key) {
		param.Remove(key);
	}

	public bool GetBool(string key) {
		System.Object o;
		param.TryGetValue(key, out o);
		return o == null? false : (bool)o;
	}

	public string GetString(string key) {
		System.Object o;
		param.TryGetValue(key, out o);
		return o == null? null : (string)o;
	}

	public int GetInt(string key) {
		System.Object o;
		param.TryGetValue(key, out o);
		return o == null? 0 : (int)o;
	}
}
