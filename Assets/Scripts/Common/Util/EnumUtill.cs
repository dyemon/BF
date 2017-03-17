public static class EnumUtill {
	
  public static IEnumerable<T> GetValues<T>() {
    return Enum.GetValues(typeof(T)).Cast<T>();
  }
}