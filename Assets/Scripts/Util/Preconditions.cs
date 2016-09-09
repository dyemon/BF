using System;

public static class Preconditions {

	public static T NotNull<T>(T obj, string error = null) {
		if(obj == null) {
			throw new System.NullReferenceException((error == null)? "Object can not be null" : error);
		}

		return obj;
	} 

	public static T NotNull<T>(T obj, string error, params Object[] param) {
		return NotNull(obj, String.Format(error, param));
	} 

	public static T Null<T>(T obj, string error = null) {
		if(obj != null) {
			throw new System.ArgumentException((error == null)? String.Format("Object %s must be null", obj) : error);
		}

		return obj;
	} 

	public static T Null<T>(T obj, string error, params Object[] param) {
		return Null<T>(obj, String.Format(error, param));
	}

	public static void Check(bool exp, string error = null) {
		if(!exp) {
			throw new ArgumentException((error == null)? "Expression is false" : error);
		}
	} 

	public static void Check(bool exp, string error, params Object[] param) {
		Check(exp, String.Format(error, param));
	}
}
