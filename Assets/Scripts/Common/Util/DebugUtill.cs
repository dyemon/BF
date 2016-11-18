using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugUtill {

	public static void Log<K,V>(IDictionary<K,V> debug, string tag = "") {
		Debug.Log("Start " + tag);

		foreach(KeyValuePair<K, V> kvp in debug) {
			Debug.Log(kvp.Key + " " + kvp.Value);
		}

		Debug.Log("End " + tag);
	}

	public static void Log<K>(LinkedList<K> debug, string tag = "") {
		Debug.Log("Start " + tag);

		foreach(K val in debug) {
			Debug.Log(val);
		}

		Debug.Log("End " + tag);
	}
}
