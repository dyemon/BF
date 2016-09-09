using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugUtill {

	public static void Log<K,V>(IDictionary<K,V> debug) {
		Debug.Log("Start");

		foreach(KeyValuePair<K, V> kvp in debug) {
			Debug.Log(kvp.Key + " " + kvp.Value);
		}

		Debug.Log("End");
	}
}
