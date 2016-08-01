using UnityEngine;
using System.Collections;

public class AIdle {

	private float time;
	private float startTime;
	private float startDurationTime;

	public AIdle(float time) {
		this.time = time;
	}

	public void Start() {
		startTime = Time.time;
		startDurationTime = time;
	}

	public bool Idle() {
		time = startDurationTime - (Time.time - startTime);
		return (time > 0);
	}
}
