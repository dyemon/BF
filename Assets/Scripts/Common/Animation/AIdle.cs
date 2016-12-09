using UnityEngine;
using System.Collections;

public class AIdle : IABase{

	private float time;
	private float startTime;
	private float startDurationTime;
	private bool isComplete = false;

	public AIdle(float time) {
		this.time = time;
	}

	public void Run() {
		startTime = Time.time;
		startDurationTime = time;
	}

	public bool Animate(GameObject go) {
		time = startDurationTime - (Time.time - startTime);
		isComplete = !(time > 0);
		return (time > 0); 
	}

	public bool IsCompleteAnimation() {
		return isComplete;
	}
}
