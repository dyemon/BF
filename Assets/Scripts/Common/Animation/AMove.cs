using UnityEngine;
using System.Collections;

public class AMove : IABase {

	private Vector3? startPos;
	private Vector3 movePos;
	private float time;
	private float speed;

	private float startTime;
	private float startDurationTime;
	private bool isComplete = false;

	public AMove(Vector3? startPos, Vector3 movePos, float speed, bool ui) {
		this.startPos = startPos;
		this.movePos = movePos;
		this.speed = speed;
		if(startPos != null) {
			time = CalcTime(startPos.Value, movePos, speed);
		}
	}

	public AMove(Vector3? startPos, Vector3 movePos) {
		this.startPos = startPos;
		this.movePos = movePos;
	}

	public static float CalcTime(Vector3 startPos, Vector3 movePos, float speed) {
		return Vector3.Distance(startPos, movePos) / speed;
	}

	public void SetTime(float time) {
		this.time = time;
	}
	public float GetTime() {
		return time;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
		if(startPos != null) {
			time = CalcTime(startPos.Value, movePos, speed);
		}	
	}
	public float GetSpeed() {
		return speed;
	}

	public void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}

	public bool Animate(GameObject gameObject) {
		if(startPos == null) {
			startPos = gameObject.transform.position;
			if(time == 0 && speed > 0) {
				time = CalcTime(startPos.Value, movePos, speed);
			}
			startDurationTime = time;
		}

		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

		gameObject.transform.position = Vector3.Lerp(startPos.Value, movePos, t);

		isComplete = !(time > 0);

		return (time > 0);
	}

	public bool IsCompleteAnimation() {
		return isComplete;
	}
}
