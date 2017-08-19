using UnityEngine;
using System.Collections;

namespace Common.Animation {
public class AResize : ABase {
	
	private Vector3? startSize;
	private Vector3 endSize;
	private float time;
	private float initTime;

	private float startTime;
	private float startDurationTime;
	private bool isComplete = false;

	public AResize(Vector3? startSize, Vector3 endSize, float time) {
		this.startSize = startSize;
		this.endSize = endSize;
		this.time = time;
		this.initTime = time;
	}

	public override void Run() {
		startTime = UnityEngine.Time.time;
		startDurationTime = time;
	}
	public override void Reset() {
		time = initTime;
	}

	public override bool Animate(GameObject gameObject) {
		if(startSize == null) {
			startSize = gameObject.transform.localScale;
		}

		float delta = UnityEngine.Time.time - startTime;
		time = startDurationTime - delta;
		float t = (time <= 0)? 1f : delta/startDurationTime;

		gameObject.transform.localScale = Vector3.Lerp(startSize.Value, endSize, t);

		isComplete = !(time > 0);
		return (time > 0); 
	}

	public override bool IsCompleteAnimation() {
		return isComplete;
	}
}
}