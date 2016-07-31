using UnityEngine;
using System.Collections;

public static class Predicates {

	public static T NotNull<T>(T obj, string error = null) {
		if(obj == null) {
			throw new System.NullReferenceException((error == null)? "Object can not be null" : error);
		}

		return obj;
	} 

	public static void Null<T>(T obj, string error = null) {
		if(obj != null) {
			throw new System.NullReferenceException((error == null)? "Object can not be null" : error);
		}
	} 
}
