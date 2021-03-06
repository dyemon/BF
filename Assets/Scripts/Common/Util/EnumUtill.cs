using System.Collections.Generic;
using System;
using System.Linq;


public static class EnumUtill {
	
  public static IEnumerable<T> GetValues<T>() {
    return Enum.GetValues(typeof(T)).Cast<T>();
  }

	public static T Parse<T>(string str) {
		return (T)Enum.Parse(typeof(T), str);
	}
}