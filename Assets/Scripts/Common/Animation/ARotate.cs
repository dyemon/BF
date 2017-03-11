using UnityEngine;
using System.Collections;

public class ARotate : IABase {

	private Vector3? startAngle;
	private Vector3 rotateAngle;
	private float time;

	private float startTime;
	private float startDurationTime;
	private bool isComplete = false;
	private Vector3? endAngle = null;

	public ARotate(Vector3? startAngle, Vector3 rotateAngle, float time) {
		this.startAngle = startAngle;
		this.rotateAngle = rotateAngle;
		this.time = time;
	}

	public void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}

	public bool Animate(GameObject gameObject) {
		if(startAngle == null) {
			startAngle = gameObject.transform.eulerAngles;
		}
		if(endAngle == null) {
			endAngle = startAngle.Value + rotateAngle;
		}

		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

	//	gameObject.transform.Rotate( Vector3.Lerp(startAngle.Value, endAngle, t));
		gameObject.transform.rotation = Quaternion.Euler( Vector3.Lerp(startAngle.Value, endAngle.Value, t));

		isComplete = !(time > 0);
		return (time > 0); 
	}

	public bool IsCompleteAnimation() {
		return isComplete;
	}
}
