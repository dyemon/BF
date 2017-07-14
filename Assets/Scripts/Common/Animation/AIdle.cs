using UnityEngine;
using System.Collections;

public class AIdle : ABase{

	private float time;
	private float startTime;
	private float startDurationTime;
	private bool isComplete = false;

	public AIdle(float time) {
		this.time = time;
	}

	public override void Run() {
		startTime = Time.time;
		startDurationTime = time;
	}

	public override bool Animate(GameObject go) {
		time = startDurationTime - (Time.time - startTime);
		isComplete = !(time > 0);
		return (time > 0); 
	}

	public override bool IsCompleteAnimation() {
		return isComplete;
	}
}
