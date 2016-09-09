using System;

public class LevelConfigException : Exception {

	public LevelConfigException() {
	}

	public LevelConfigException(string message)
		: base(message) {
	}

	public LevelConfigException(string message, Exception inner)
		: base(message, inner) {
	}
}
