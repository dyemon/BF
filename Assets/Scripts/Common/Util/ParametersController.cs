using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersController {
	public static ParametersController Instance = new ParametersController();
	private IDictionary<string, System.Object> param = new  Dictionary<string, System.Object>();

	public const string CAPITULATE_NOT_ENDED = "CAPITULATE_NOT_ENDED";
	public const string CAN_SHOW_DAILYBONUS = "CAN_SHOW_DAILYBONUS";
	public const string CANSEL_DAILYBONUS = "CANSEL_DAILYBONUS";
	public const string CAN_SHOW_FORTUNA = "CAN_SHOW_FORTUNA";
	public const string FORTUNA_IS_SHOWN = "FORTUNA_IS_SHOWN";
	public const string RECEIVED_GIFT_CACHE_UPDATED = "RECEIVED_GIFT_CACHE_UPDATED";

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
